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

        public IEnumerable<int> GetEnqueuedJobIds(string queue, int @from, int perPage, Pager pager = null)
        {
            dynamic parameters = null;
            if (pager != null)
            {
                parameters = new
                {
                    startDate = pager.JobsFilterStartDate,
                    endDate = pager.JobsFilterEndDate,
                    startTime = pager.JobsFilterStartTime,
                    endTime = pager.JobsFilterEndTime
                };
            }

            string[] sqlFilterDates = prepareSqlFilterDates(parameters);

            string[] queryParams = new string[]
            {
                  _storage.GetSchemaName(),
                  pager == null || string.IsNullOrEmpty(pager.JobsFilterText) ? string.Empty : " and j.Arguments LIKE '%'+@filterString+'%' ",
                  pager == null || string.IsNullOrEmpty(pager.JobsFilterMethodText) ? string.Empty : " and J.InvocationData LIKE '%\"Method\":\"%'+@filterMethodString+'%\",\"ParameterTypes\"%' ",
                  !hasDateTimeParams(parameters) ? string.Empty : " and @startDate <= j.CreatedAt and j.CreatedAt <= @endDate "
            };
           
            string sqlQuery = string.Format(@"
select distinct r.JobId from (
  select jq.JobId, row_number() over (order by jq.Id) as row_num 
  from [{0}].JobQueue as jq, [{0}].Job as j 
  where jq.Queue = @queue AND jq.JobId = j.Id {1} {2} {3}  
) as r
where r.row_num between @start and @end", queryParams);

            var filterString = "";
            var filterMethodString = "";
            if (pager != null)
            {
                filterString = pager.JobsFilterText;
                filterMethodString = pager.JobsFilterMethodText;
            }

            return UseTransaction(connection =>
            {
                return connection.Query<JobIdDto>(
                    sqlQuery,
                    new { queue = queue, start = from + 1, end = @from + perPage, filterString = filterString, filterMethodString = filterMethodString, startDate = sqlFilterDates[0], endDate =sqlFilterDates[1] })
                    .ToList()
                    .Select(x => x.JobId)
                    .ToList();
            });
        }

        public IEnumerable<int> GetFetchedJobIds(string queue, int @from, int perPage)
        {
            return Enumerable.Empty<int>();
        }

        public EnqueuedAndFetchedCountDto GetEnqueuedAndFetchedCount(string queue, Dictionary<string,string> countParameters = null)
        {
            string[] sqlFilterDates = prepareSqlFilterDates(countParameters);
            
            string[] queryParams = queryParams = new string[]
            {
                  _storage.GetSchemaName(),                  
                  countParameters == null || countParameters.Count <= 0 || string.IsNullOrEmpty(countParameters["filterString"]) ? string.Empty : " and j.Arguments LIKE '%'+@filterString+'%' ",
                  countParameters == null || countParameters.Count <= 0 || string.IsNullOrEmpty(countParameters["filterMethodString"]) ? string.Empty : " and J.InvocationData LIKE '%\"Method\":\"%'+@filterMethodString+'%\",\"ParameterTypes\"%' ",
                  !hasDateTimeParams(countParameters) ? string.Empty : " and @startDate <= j.CreatedAt and j.CreatedAt <= @endDate "
            };
            
            string sqlQuery = string.Format(@"
    select count(jq.Id) 
    from [{0}].JobQueue as jq, [{0}].Job as j 
    where jq.Queue = @queue and jq.JobId = j.Id {1} {2} {3} ", queryParams);

            var filterString = "";
            var filterMethodString = "";
            if (countParameters != null)
            {
                filterString = countParameters["filterString"];
                filterMethodString = countParameters["filterMethodText"];
            }

            return UseTransaction(connection =>
            {
                var result = connection.Query<int>(sqlQuery, new { queue = queue,
                    filterString = filterString,
                    filterMethodString = filterMethodString,
                    startDate = sqlFilterDates[0],
                    endDate = sqlFilterDates[1] })
                    .Single();

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

        private string[] prepareSqlFilterDates(Dictionary<string,string> parameters)
        {
            if ( parameters == null || parameters.Count <= 0 || !hasDateTimeParams(parameters) ) return new string[] { string.Empty, string.Empty };

            var startDate = parameters["startDate"];
            var endDate = parameters["endDate"];
            var startTime = parameters["startTime"];
            var endTime = parameters["endTime"];

            string start, end;
            var startDateData = startDate.Split('-');
            var endDateData = endDate.Split('-');
            var startTimeData = startTime.Split('-');
            var endTimeData = endTime.Split('-');

            start = new DateTime(int.Parse(startDateData[2]), int.Parse(startDateData[1]), int.Parse(startDateData[0]), int.Parse(startTimeData[0]), int.Parse(startTimeData[1]), 0).ToString("yyyy-MM-dd HH:mm:ss");
            end = new DateTime(int.Parse(endDateData[2]), int.Parse(endDateData[1]), int.Parse(endDateData[0]), int.Parse(endTimeData[0]), int.Parse(endTimeData[1]), 0).ToString("yyyy-MM-dd HH:mm:ss");

            return new string[] { start, end };
        }

        private bool hasDateTimeParams(Dictionary<string,string> parameters)
        {
            if (parameters == null || parameters.Count <= 0) return false;

            var startDate = parameters["startDate"];
            var endDate = parameters["endDate"];
            var startTime = parameters["startTime"];
            var endTime = parameters["endTime"];

            return !(string.IsNullOrEmpty(startDate) ||
                string.IsNullOrEmpty(endDate) ||
                string.IsNullOrEmpty(startTime) ||
                string.IsNullOrEmpty(endTime));
        }
    }
}