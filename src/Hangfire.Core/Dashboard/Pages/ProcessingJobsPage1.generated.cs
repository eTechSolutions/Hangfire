﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Hangfire.Dashboard.Pages
{
    
    #line 2 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
    using System;
    
    #line default
    #line hidden
    using System.Collections.Generic;
    
    #line 3 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
    using System.Linq;
    
    #line default
    #line hidden
    using System.Text;
    
    #line 4 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
    using Hangfire.Dashboard;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
    using Hangfire.Dashboard.Pages;
    
    #line default
    #line hidden
    
    #line 6 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
    using Hangfire.States;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    internal partial class ProcessingJobsPage : RazorPage
    {
#line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");








            
            #line 8 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
  
    Layout = new LayoutPage("Processing Jobs");

    int from, perPage;

    int.TryParse(Query("from"), out from);
    int.TryParse(Query("count"), out perPage);
    string filterString = Query("filterString");
    string filterMethodString = Query("filterMethodString");
    string startDate = Query("startDate");
    string endDate = Query("endDate");
    string startTime = Query("startTime");
    string endTime = Query("endTime");

    var monitor = Storage.GetMonitoringApi();
    var countParameters = new Dictionary<string, string>()
    {
        { "stateName", ProcessingState.StateName },
        { "filterString", filterString },
        { "filterMethodString", filterMethodString },
        { "startDate", startDate },
        { "endDate", endDate },
        { "startTime", startTime },
        { "endTime", endTime }
    };

    var jobCount = monitor.JobCountByStateName(countParameters);
    var pager = new Pager(from, perPage, jobCount)
    {
        JobsFilterText = filterString,
        JobsFilterMethodText = filterMethodString,
        JobsFilterStartDate = startDate,
        JobsFilterEndDate = endDate,
        JobsFilterStartTime = startTime,
        JobsFilterEndTime = endTime
    };

    var processingJobs = monitor.ProcessingJobs(pager);
    var servers = monitor.Servers();


            
            #line default
            #line hidden
WriteLiteral("\r\n<div class=\"row\">\r\n    <div class=\"col-md-3\">\r\n        ");


            
            #line 51 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
   Write(Html.JobsSidebar());

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div class=\"col-md-9\">\r\n        <h1 class=\"page-header\">Process" +
"ing Jobs</h1>\r\n\r\n");


            
            #line 56 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
         if (pager.TotalPageCount == 0)
        {

            
            #line default
            #line hidden
WriteLiteral("            <div class=\"alert alert-info\">\r\n                No jobs are being pro" +
"cessed right now.\r\n            </div>\r\n");


            
            #line 61 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
        }
        else
        {

            
            #line default
            #line hidden
WriteLiteral("            <div class=\"js-jobs-list\">\r\n                <div class=\"btn-toolbar b" +
"tn-toolbar-top\">\r\n                    <button class=\"js-jobs-list-command btn bt" +
"n-sm btn-primary\"\r\n                            data-url=\"");


            
            #line 67 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                 Write(Url.To("/jobs/processing/requeue"));

            
            #line default
            #line hidden
WriteLiteral(@"""
                            data-loading-text=""Enqueueing...""
                            disabled=""disabled"">
                        <span class=""glyphicon glyphicon-repeat""></span>
                        Requeue jobs
                    </button>

                    <button class=""js-jobs-list-command btn btn-sm btn-default""
                            data-url=""");


            
            #line 75 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                 Write(Url.To("/jobs/processing/delete"));

            
            #line default
            #line hidden
WriteLiteral(@"""
                            data-loading-text=""Deleting...""
                            data-confirm=""Do you really want to DELETE ALL selected jobs?""
                            disabled=""disabled"">
                        <span class=""glyphicon glyphicon-remove""></span>
                        Delete selected
                    </button>
                    <br>
                    <br>
                    <button class=""js-jobs-filter-command btn btn-sm btn-default""
                            data-url=""");


            
            #line 85 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                 Write(Url.To("/jobs/processing"));

            
            #line default
            #line hidden
WriteLiteral(@""">
                        <span class=""glyphicon glyphicon-repeat""></span>
                        Filter jobs
                    </button>
                    <input type=""text"" value="""" id=""filterValueString"" class=""fltr-txtbx btn-sm"" />
                    <div style=""width:200px;margin-right:10px;"">
                        <input id=""filterOnDateTime"" type=""checkbox"" class=""js-jobs-filterOnDateTime-checked"">");


            
            #line 91 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                                                                                         Write(Html.Raw("Filter on date and time"));

            
            #line default
            #line hidden
WriteLiteral("</input>\r\n                        <br />\r\n                        <input id=\"filt" +
"erOnMethodName\" type=\"checkbox\" class=\"js-jobs-filterOnMethodName-checked\">");


            
            #line 93 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                                                                                             Write(Html.Raw("Filter on method name"));

            
            #line default
            #line hidden
WriteLiteral("</input>\r\n                    </div>\r\n");


            
            #line 95 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                      
                        var currentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    

            
            #line default
            #line hidden
WriteLiteral(@"                    <br />
                    <br />
                    <label id=""methodValueLabel"" hidden=""hidden"">Method name</label>
                    <input type=""text"" value="""" id=""filterMethodString"" class=""fltr-txtbx btn-sm"" style=""padding-bottom:inherit"" hidden=""hidden"" />
                    <br />
                    <input type=""text"" value=""");


            
            #line 103 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                         Write(currentDateTime);

            
            #line default
            #line hidden
WriteLiteral("\" style=\"width: 130px; margin:0px 0px 0px 80px;\" class=\"datetimeselector-start\" i" +
"d=\"startDateTime\" hidden=\"hidden\" />\r\n                    <input type=\"text\" val" +
"ue=\"");


            
            #line 104 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                         Write(currentDateTime);

            
            #line default
            #line hidden
WriteLiteral("\" style=\"width: 130px;\" class=\"datetimeselector-end\" id=\"endDateTime\" hidden=\"hid" +
"den\" />\r\n                    ");


            
            #line 105 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
               Write(Html.PerPageSelector(pager));

            
            #line default
            #line hidden
WriteLiteral(@"
                </div>

                <table class=""table"">
                    <thead>
                    <tr>
                        <th class=""min-width"">
                            <input type=""checkbox"" class=""js-jobs-list-select-all""/>
                        </th>
                        <th class=""min-width"">Id</th>
                        <th class=""min-width"">Server</th>
                        <th>Job</th>
                        <th class=""align-right"">Started</th>
                    </tr>
                    </thead>
                    <tbody>
");


            
            #line 121 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                     foreach (var job in processingJobs)
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <tr class=\"js-jobs-list-row ");


            
            #line 123 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                                Write(!job.Value.InProcessingState ? "obsolete-data" : null);

            
            #line default
            #line hidden
WriteLiteral(" ");


            
            #line 123 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                                                                                         Write(job.Value.InProcessingState ? "hover" : null);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n                            <td>\r\n");


            
            #line 125 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                 if (job.Value.InProcessingState)
                                {

            
            #line default
            #line hidden
WriteLiteral("                                    <input type=\"checkbox\" class=\"js-jobs-list-ch" +
"eckbox\" name=\"jobs[]\" value=\"");


            
            #line 127 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                                                                                         Write(job.Key);

            
            #line default
            #line hidden
WriteLiteral("\"/>\r\n");


            
            #line 128 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                }

            
            #line default
            #line hidden
WriteLiteral("                            </td>\r\n                            <td class=\"min-wid" +
"th\">\r\n                                ");


            
            #line 131 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                           Write(Html.JobIdLink(job.Key));

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 132 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                 if (!job.Value.InProcessingState)
                                {

            
            #line default
            #line hidden
WriteLiteral("                                    <span title=\"Job\'s state has been changed whi" +
"le fetching data.\" class=\"glyphicon glyphicon-question-sign\"></span>\r\n");


            
            #line 135 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                }

            
            #line default
            #line hidden
WriteLiteral("                            </td>\r\n                            <td class=\"min-wid" +
"th\">\r\n                                ");


            
            #line 138 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                           Write(job.Value.ServerId.ToUpperInvariant());

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </td>\r\n                            <td>\r\n");


            
            #line 141 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                 if (servers.All(x => x.Name != job.Value.ServerId || x.Heartbeat < DateTime.UtcNow.AddMinutes(-1)))
                                {

            
            #line default
            #line hidden
WriteLiteral("                                    <span title=\"Looks like the job was aborted\" " +
"class=\"glyphicon glyphicon-warning-sign\" style=\"font-size: 10px;\"></span>\r\n");


            
            #line 144 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                }

            
            #line default
            #line hidden
WriteLiteral("                                \r\n                                ");


            
            #line 146 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                           Write(Html.JobNameLink(job.Key, job.Value.Job));

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </td>\r\n                            <td class=\"align" +
"-right\">\r\n");


            
            #line 149 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                 if (job.Value.StartedAt.HasValue)
                                {
                                    
            
            #line default
            #line hidden
            
            #line 151 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                               Write(Html.Raw(job.Value.StartedAt.Value.ToString("dd/MM/yyyy HH:mm")));

            
            #line default
            #line hidden
            
            #line 151 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                                                                                                     ;
                                }

            
            #line default
            #line hidden
WriteLiteral("                            </td>\r\n                        </tr>\r\n");


            
            #line 155 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                    </tbody>\r\n                </table>\r\n\r\n                ");


            
            #line 159 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
           Write(Html.Paginator(pager));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n");


            
            #line 161 "..\..\Dashboard\Pages\ProcessingJobsPage.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n</div>");


        }
    }
}
#pragma warning restore 1591
