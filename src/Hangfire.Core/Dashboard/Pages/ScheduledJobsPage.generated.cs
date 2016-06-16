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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    #line 2 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
    using Hangfire.Dashboard;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
    using Hangfire.Dashboard.Pages;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
    using Hangfire.States;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    internal partial class ScheduledJobsPage : RazorPage
    {
#line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");






            
            #line 6 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
  
    Layout = new LayoutPage("Scheduled Jobs");

    int from, perPage;

    int.TryParse(Query("from"), out from);
    int.TryParse(Query("count"), out perPage);
    string filterString = Query("filterString");
    string startDate = Query("startDate");
    string endDate = Query("endDate");

    var monitor = Storage.GetMonitoringApi();
    var pager = new Pager(from, perPage, monitor.JobCountByStateName(ScheduledState.StateName, filterString,startDate,endDate), filterString, startDate, endDate);
    var scheduledJobs = monitor.ScheduledJobs(pager);


            
            #line default
            #line hidden
WriteLiteral("\r\n<div class=\"row\">\r\n    <div class=\"col-md-3\">\r\n        ");


            
            #line 24 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
   Write(Html.JobsSidebar());

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div class=\"col-md-9\">\r\n        <h1 class=\"page-header\">Schedul" +
"ed Jobs</h1>\r\n\r\n");


            
            #line 29 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
         if (pager.TotalPageCount == 0)
        {

            
            #line default
            #line hidden
WriteLiteral("            <div class=\"alert alert-info\">\r\n                There are no schedule" +
"d jobs.\r\n            </div>\r\n");


            
            #line 34 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
        }
        else
        {

            
            #line default
            #line hidden
WriteLiteral("            <div class=\"js-jobs-list\">\r\n                <div class=\"btn-toolbar b" +
"tn-toolbar-top\">\r\n                    <button class=\"js-jobs-list-command btn bt" +
"n-sm btn-primary\"\r\n                            data-url=\"");


            
            #line 40 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                 Write(Url.To("/jobs/scheduled/enqueue"));

            
            #line default
            #line hidden
WriteLiteral(@"""
                            data-loading-text=""Enqueueing...""
                            disabled=""disabled"">
                        <span class=""glyphicon glyphicon-play""></span>
                        Enqueue now
                    </button>

                    <button class=""js-jobs-list-command btn btn-sm btn-default""
                            data-url=""");


            
            #line 48 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                 Write(Url.To("/jobs/scheduled/delete"));

            
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


            
            #line 58 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                 Write(Url.To("/jobs/failed"));

            
            #line default
            #line hidden
WriteLiteral(@""">
                        <span class=""glyphicon glyphicon-repeat""></span>
                        Filter jobs
                    </button>
                    <input type=""text"" value="""" id=""filterValueString"" class=""js-jobs-filtertext-clear fltr-txtbx btn-sm"" />
                    <input id=""filterOnDate"" type=""checkbox"" class=""js-jobs-filterOnDate-checked"">Filter on date</input>
");


            
            #line 64 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                      
                        var todaysDate = System.DateTime.Now.ToShortDateString();
                    

            
            #line default
            #line hidden
WriteLiteral("                    <br />\r\n                    <br />\r\n                    <inpu" +
"t value=\"");


            
            #line 69 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                             Write(todaysDate);

            
            #line default
            #line hidden
WriteLiteral("\" style=\"width: 100px; margin:0px 0px 0px 110px;\" readonly=\"\" class=\"dateselector" +
"-start\" id=\"startDate\" hidden=\"hidden\" />\r\n                    <input value=\"");


            
            #line 70 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                             Write(todaysDate);

            
            #line default
            #line hidden
WriteLiteral("\" style=\"width: 100px;\" readonly=\"\" class=\"dateselector-end\" id=\"endDate\" hidden=" +
"\"hidden\" />\r\n                    ");


            
            #line 71 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
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
                        <th>Enqueue</th>
                        <th>Job</th>
                        <th class=""align-right"">Scheduled</th>
                    </tr>
                    </thead>
");


            
            #line 86 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                     foreach (var job in scheduledJobs)
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <tr class=\"js-jobs-list-row ");


            
            #line 88 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                                Write(!job.Value.InScheduledState ? "obsolete-data" : null);

            
            #line default
            #line hidden
WriteLiteral(" ");


            
            #line 88 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                                                                                        Write(job.Value.InScheduledState ? "hover" : null);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n                            <td>\r\n");


            
            #line 90 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                 if (job.Value.InScheduledState)
                                {

            
            #line default
            #line hidden
WriteLiteral("                                    <input type=\"checkbox\" class=\"js-jobs-list-ch" +
"eckbox\" name=\"jobs[]\" value=\"");


            
            #line 92 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                                                                                         Write(job.Key);

            
            #line default
            #line hidden
WriteLiteral("\"/>\r\n");


            
            #line 93 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                }

            
            #line default
            #line hidden
WriteLiteral("                            </td>\r\n                            <td class=\"min-wid" +
"th\">\r\n                                ");


            
            #line 96 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                           Write(Html.JobIdLink(job.Key));

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 97 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                 if (!job.Value.InScheduledState)
                                {

            
            #line default
            #line hidden
WriteLiteral("                                    <span title=\"Job\'s state has been changed whi" +
"le fetching data.\" class=\"glyphicon glyphicon-question-sign\"></span>\r\n");


            
            #line 100 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                }

            
            #line default
            #line hidden
WriteLiteral("                            </td>\r\n                            <td class=\"min-wid" +
"th\">\r\n                                ");


            
            #line 103 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                           Write(Html.RelativeTime(job.Value.EnqueueAt));

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </td>\r\n                            <td>\r\n          " +
"                      ");


            
            #line 106 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                           Write(Html.JobNameLink(job.Key, job.Value.Job));

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </td>\r\n                            <td class=\"align" +
"-right\">\r\n");


            
            #line 109 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                 if (job.Value.ScheduledAt.HasValue)
                                {
                                    
            
            #line default
            #line hidden
            
            #line 111 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                               Write(Html.RelativeTime(job.Value.ScheduledAt.Value));

            
            #line default
            #line hidden
            
            #line 111 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                                                                   
                                }

            
            #line default
            #line hidden
WriteLiteral("                            </td>\r\n                        </tr>\r\n");


            
            #line 115 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                </table>\r\n\r\n                ");


            
            #line 118 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
           Write(Html.Paginator(pager));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n");


            
            #line 120 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n</div>");


        }
    }
}
#pragma warning restore 1591
