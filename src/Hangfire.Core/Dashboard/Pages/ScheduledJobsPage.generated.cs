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
    string filterMethodString = Query("filterMethodString");
    string startDate = Query("startDate");
    string endDate = Query("endDate");
    string startTime = Query("startTime");
    string endTime = Query("endTime");

    var monitor = Storage.GetMonitoringApi();
    var countParameters = new Dictionary<string, string>()
    {
        { "stateName", ScheduledState.StateName },
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

    var scheduledJobs = monitor.ScheduledJobs(pager);


            
            #line default
            #line hidden
WriteLiteral("\r\n<div class=\"row\">\r\n    <div class=\"col-md-3\">\r\n        ");


            
            #line 48 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
   Write(Html.JobsSidebar());

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div class=\"col-md-9\">\r\n        <h1 class=\"page-header\">Schedul" +
"ed Jobs</h1>\r\n\r\n");


            
            #line 53 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
         if (pager.TotalPageCount == 0)
        {

            
            #line default
            #line hidden
WriteLiteral("            <div class=\"alert alert-info\">\r\n                There are no schedule" +
"d jobs.\r\n            </div>\r\n");


            
            #line 58 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
        }
        else
        {

            
            #line default
            #line hidden
WriteLiteral("            <div class=\"js-jobs-list\">\r\n                <div class=\"btn-toolbar b" +
"tn-toolbar-top\">\r\n                    <button class=\"js-jobs-list-command btn bt" +
"n-sm btn-primary\"\r\n                            data-url=\"");


            
            #line 64 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
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


            
            #line 71 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
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
                    <button data-toggle=""collapse"" data-target=""#advanced-search-bar"" class=""btn btn-sm btn-success"">Advanced Search</button>
                </div>
                <div id=""advanced-search-bar"" class=""collapse well"">
                    <h4 class=""advanced-search-header"">
                        Advanced Search
                    </h4>
                    <div class=""row"">
                        <div class=""col-md-12"">
                            <div class=""form-group"">
                                <input type=""text"" value="""" id=""filterValueString"" class=""form-control"" placeholder=""Search..."" />
                            </div>
                            <div class=""form-group"">
");


            
            #line 90 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                  
                                    var currentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                                

            
            #line default
            #line hidden
WriteLiteral(@"                                <input id=""filterOnDateTime"" name=""filterOnDateTime"" type=""checkbox"" class=""js-jobs-filterOnDateTime-checked"" />
                                <label for=""filterOnDateTime"">Filter on date time</label>
                                <div id=""datetime-filters"" class=""row"">
                                    <div class=""col-xs-6"">
                                        <input type=""text"" value=""");


            
            #line 97 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                                             Write(currentDateTime);

            
            #line default
            #line hidden
WriteLiteral("\" class=\"datetimeselector-start form-control\" id=\"startDateTime\" />\r\n            " +
"                        </div>\r\n                                    <div class=\"" +
"col-xs-6\">\r\n                                        <input type=\"text\" value=\"");


            
            #line 100 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                                             Write(currentDateTime);

            
            #line default
            #line hidden
WriteLiteral(@""" class=""datetimeselector-end form-control"" id=""endDateTime"" />
                                    </div>
                                </div>
                            </div>
                            <div class=""form-group"">
                                <input id=""filterOnMethodName"" name=""filterOnMethodName"" type=""checkbox"" class=""js-jobs-filterOnMethodName-checked"" />
                                <label for=""filterOnMethodName"">Filter on method name</label>
                                <input type=""text"" value="""" id=""filterMethodString"" class=""form-control"" placeholder=""Method name..."" />
                            </div>
                        </div>
                    </div>
                    <div class=""row"">
                        <div class=""col-md-12"">
                            <button class=""js-jobs-filter-command btn btn-sm btn-success"" data-url=""");


            
            #line 113 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                                                                               Write(Url.To("/jobs/scheduled"));

            
            #line default
            #line hidden
WriteLiteral(@""">
                                <span class=""glyphicon glyphicon-repeat""></span>
                                Filter jobs
                            </button>
                        </div>
                    </div>
                </div>
                ");


            
            #line 120 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
           Write(Html.PerPageSelector(pager));

            
            #line default
            #line hidden
WriteLiteral(@"
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


            
            #line 133 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                     foreach (var job in scheduledJobs)
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <tr class=\"js-jobs-list-row ");


            
            #line 135 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                                Write(!job.Value.InScheduledState ? "obsolete-data" : null);

            
            #line default
            #line hidden
WriteLiteral(" ");


            
            #line 135 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                                                                                        Write(job.Value.InScheduledState ? "hover" : null);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n                            <td>\r\n");


            
            #line 137 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                 if (job.Value.InScheduledState)
                                {

            
            #line default
            #line hidden
WriteLiteral("                                    <input type=\"checkbox\" class=\"js-jobs-list-ch" +
"eckbox\" name=\"jobs[]\" value=\"");


            
            #line 139 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                                                                                         Write(job.Key);

            
            #line default
            #line hidden
WriteLiteral("\"/>\r\n");


            
            #line 140 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                }

            
            #line default
            #line hidden
WriteLiteral("                            </td>\r\n                            <td class=\"min-wid" +
"th\">\r\n                                ");


            
            #line 143 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                           Write(Html.JobIdLink(job.Key));

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 144 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                 if (!job.Value.InScheduledState)
                                {

            
            #line default
            #line hidden
WriteLiteral("                                    <span title=\"Job\'s state has been changed whi" +
"le fetching data.\" class=\"glyphicon glyphicon-question-sign\"></span>\r\n");


            
            #line 147 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                }

            
            #line default
            #line hidden
WriteLiteral("                            </td>\r\n                            <td class=\"min-wid" +
"th\">\r\n                                ");


            
            #line 150 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                           Write(Html.Raw(job.Value.EnqueueAt.ToString("dd/MM/yyyy HH:mm")));

            
            #line default
            #line hidden
WriteLiteral(";\r\n                            </td>\r\n                            <td>\r\n         " +
"                       ");


            
            #line 153 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                           Write(Html.JobNameLink(job.Key, job.Value.Job));

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </td>\r\n                            <td class=\"align" +
"-right\">\r\n");


            
            #line 156 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                 if (job.Value.ScheduledAt.HasValue)
                                {
                                    
            
            #line default
            #line hidden
            
            #line 158 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                               Write(Html.Raw(job.Value.ScheduledAt.Value.ToString("dd/MM/yyyy HH:mm")));

            
            #line default
            #line hidden
            
            #line 158 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                                                                                                       ;
                                }

            
            #line default
            #line hidden
WriteLiteral("                            </td>\r\n                        </tr>\r\n");


            
            #line 162 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                </table>\r\n\r\n                ");


            
            #line 165 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
           Write(Html.Paginator(pager));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n");


            
            #line 167 "..\..\Dashboard\Pages\ScheduledJobsPage.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n</div>");


        }
    }
}
#pragma warning restore 1591
