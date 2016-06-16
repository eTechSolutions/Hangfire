// This file is part of Hangfire.
// Copyright © 2013-2014 Sergey Odinokov.
// 
// Hangfire is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as 
// published by the Free Software Foundation, either version 3 
// of the License, or any later version.
// 
// Hangfire is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public 
// License along with Hangfire. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using Dapper;
using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Hangfire.SqlServer
{
    internal class SqlServerJobQueueMonitoringApi : IPersistentJobQueueMonitoringApi
    {
        private static readonly TimeSpan QueuesCacheTimeout = TimeSpan.FromSeconds(5);

        private readonly SqlServerStorage _storage;
        private readonly object _cacheLock = new object();

        private List<string> _queuesCache = new List<string>();
        private DateTime _cacheUpdated;

        public SqlServerJobQueueMonitoringApi([NotNull] SqlServerStorage storage)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            _storage = storage;
        }

        public IEnumerable<string> GetQueues()
        {
            string sqlQuery = string.Format(@"select distinct(Queue) from [{0}].JobQueue", _storage.GetSchemaName());

            lock (_cacheLock)
            {
                if (_queuesCache.Count == 0 || _cacheUpdated.Add(QueuesCacheTimeout) < DateTime.UtcNow)
                {
                    var result = UseTransaction(connection =>
                    {
                        return connection.Query(sqlQuery).Select(x => (string) x.Queue).ToList();
                    });

                    _queuesCache = result;
                    _cacheUpdated = DateTime.UtcNow;
                }

                return _queuesCache.ToList();
            }  
        }

        public IEnumerable<int> GetEnqueuedJobIds(string queue, int @from, int perPage, string filterString = null, string startDate = null, string endDate = null)
        {
            string[] sqlFilterDates = prepareSqlFilterDates(startDate, endDate);

            string[] queryParams = new string[]
            {
                  _storage.GetSchemaName(),
                  string.IsNullOrEmpty(filterString) ? string.Empty : " and j.Arguments LIKE '%'+@filterString+'%' ",
                  string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate) ? string.Empty : " and @startDate <= j.CreatedAt and j.CreatedAt <= @endDate "
            };
           
            string sqlQuery = string.Format(@"
select distinct r.JobId from (
  select jq.JobId, row_number() over (order by jq.Id) as row_num 
  from [{0}].JobQueue as jq, [{0}].Job as j 
  where jq.Queue = @queue AND jq.JobId = j.Id {1} {2}  
) as r
where r.row_num between @start and @end", queryParams);

            return UseTransaction(connection =>
            {
                return connection.Query<JobIdDto>(
                    sqlQuery,
                    new { queue = queue, start = from + 1, end = @from + perPage, filterString = filterString, startDate = sqlFilterDates[0], endDate =sqlFilterDates[1] })
                    .ToList()
                    .Select(x => x.JobId)
                    .ToList();
            });
        }

        public IEnumerable<int> GetFetchedJobIds(string queue, int @from, int perPage)
        {
            return Enumerable.Empty<int>();
        }

        public EnqueuedAndFetchedCountDto GetEnqueuedAndFetchedCount(string queue, string filterString = null, string startDate = null, string endDate = null)
        {
            string[] sqlFilterDates = prepareSqlFilterDates(startDate, endDate);

            bool hasFilterValues = string.IsNullOrEmpty(filterString) && string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate);

            string[] queryParams = queryParams = new string[]
            {
                  _storage.GetSchemaName(),
                  string.IsNullOrEmpty(filterString) ? string.Empty : " and j.Arguments LIKE '%'+@filterString+'%' ",
                  string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate) ? string.Empty : " and @startDate <= j.CreatedAt and j.CreatedAt <= @endDate "
            };
            
            string sqlQuery = string.Format(@"
    select count(jq.Id) 
    from [{0}].JobQueue as jq, [{0}].Job as j 
    where jq.Queue = @queue and jq.JobId = j.Id {1} {2} ", queryParams);
                        
            return UseTransaction(connection =>
            {
                var result = connection.Query<int>(sqlQuery, new { queue = queue, filterString = filterString, startDate = sqlFilterDates[0], endDate = sqlFilterDates[1] }).Single();

                return new EnqueuedAndFetchedCountDto
                {
                    EnqueuedCount = result,
                };
            });
        }

        private T UseTransaction<T>(Func<SqlConnection, T> func)
        {
            return _storage.UseTransaction(func, IsolationLevel.ReadUncommitted);
        }

        private class JobIdDto
        {
            [UsedImplicitly]
            public int JobId { get; set; }
        }

        private string[] prepareSqlFilterDates([NotNull]string startDate, [NotNull]string endDate)
        {
            if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate)) return new string[] { string.Empty, string.Empty };

            string start, end;
            var startDateData = startDate.Split('-');
            var endDateData = endDate.Split('-');

            if (startDate == endDate)
            {
                start = new DateTime(int.Parse(startDateData[2]), int.Parse(startDateData[1]), int.Parse(startDateData[0]), 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
                end = new DateTime(int.Parse(endDateData[2]), int.Parse(endDateData[1]), int.Parse(endDateData[0]), 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                start = new DateTime(int.Parse(startDateData[2]), int.Parse(startDateData[1]), int.Parse(startDateData[0]), 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
                end = new DateTime(int.Parse(endDateData[2]), int.Parse(endDateData[1]), int.Parse(endDateData[0]), 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
            }

            return new string[] { start, end };
        }        
    }
}