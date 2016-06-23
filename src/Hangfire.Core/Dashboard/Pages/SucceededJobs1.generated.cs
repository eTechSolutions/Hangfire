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
    
    #line 2 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
    using System;
    
    #line default
    #line hidden
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    #line 3 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
    using Hangfire.Dashboard;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
    using Hangfire.Dashboard.Pages;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
    using Hangfire.States;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    internal partial class SucceededJobs : RazorPage
    {
#line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");







            
            #line 7 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
  
    Layout = new LayoutPage("Succeeded Jobs");

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
        { "stateName", SucceededState.StateName },
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

    var succeededJobs = monitor.SucceededJobs(pager);


            
            #line default
            #line hidden
WriteLiteral("\r\n<div class=\"row\">\r\n    <div class=\"col-md-3\">\r\n        ");


            
            #line 49 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
   Write(Html.JobsSidebar());

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div class=\"col-md-9\">\r\n        <h1 class=\"page-header\">Succeed" +
"ed Jobs</h1>\r\n\r\n");


            
            #line 54 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
         if (pager.TotalPageCount == 0)
        {

            
            #line default
            #line hidden
WriteLiteral("            <div class=\"alert alert-info\">\r\n                No succeeded jobs fou" +
"nd.\r\n            </div>\r\n");


            
            #line 59 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
        }
        else
        {

            
            #line default
            #line hidden
WriteLiteral("            <div class=\"js-jobs-list\">\r\n                <div class=\"btn-toolbar b" +
"tn-toolbar-top\">\r\n                    <button class=\"js-jobs-list-command btn bt" +
"n-sm btn-primary\"\r\n                            data-url=\"");


            
            #line 65 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                 Write(Url.To("/jobs/succeeded/requeue"));

            
            #line default
            #line hidden
WriteLiteral(@"""
                            data-loading-text=""Enqueueing...""
                            disabled=""disabled"">
                        <span class=""glyphicon glyphicon-repeat""></span>
                        Requeue jobs
                    </button>
                    <br>
                    <br>
                    <button class=""js-jobs-filter-command btn btn-sm btn-default""
                            data-url=""");


            
            #line 74 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                 Write(Url.To("/jobs/succeeded"));

            
            #line default
            #line hidden
WriteLiteral(@""">
                        <span class=""glyphicon glyphicon-repeat""></span>
                        Filter jobs
                    </button>
                    <input type=""text"" value="""" id=""filterValueString"" class=""fltr-txtbx btn-sm"" />
                    <div style=""width:200px;margin-right:10px;"">
                        <input id=""filterOnDateTime"" type=""checkbox"" class=""js-jobs-filterOnDateTime-checked"">");


            
            #line 80 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                                                                                         Write(Html.Raw("Filter on date and time"));

            
            #line default
            #line hidden
WriteLiteral("</input>\r\n                        <br />\r\n                        <input id=\"filt" +
"erOnMethodName\" type=\"checkbox\" class=\"js-jobs-filterOnMethodName-checked\">");


            
            #line 82 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                                                                                             Write(Html.Raw("Filter on method name"));

            
            #line default
            #line hidden
WriteLiteral("</input>\r\n                    </div>\r\n");


            
            #line 84 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                      
                        var todaysDate = System.DateTime.Now.ToShortDateString();
                    

            
            #line default
            #line hidden
WriteLiteral(@"                    <br />
                    <br />
                    <label id=""methodValueLabel"" hidden=""hidden"">Method name</label>
                    <input type=""text"" value="""" id=""filterMethodString"" class=""fltr-txtbx btn-sm"" style=""padding-bottom:inherit"" hidden=""hidden"" />
                    <br />
                    <input value=""");


            
            #line 92 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                             Write(todaysDate);

            
            #line default
            #line hidden
WriteLiteral("\" style=\"width: 100px; margin:0px 0px 0px 110px;\" readonly=\"\" class=\"dateselector" +
"-start\" id=\"startDate\" hidden=\"hidden\" />\r\n                    <input value=\"");


            
            #line 93 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                             Write(todaysDate);

            
            #line default
            #line hidden
WriteLiteral("\" style=\"width: 100px;\" readonly=\"\" class=\"dateselector-end\" id=\"endDate\" hidden=" +
"\"hidden\" />\r\n                    ");


            
            #line 94 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
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
                        <th>Job</th>
                        <th class=""min-width"">Total Duration</th>
                        <th class=""align-right"">Succeeded</th>
                    </tr>
                    </thead>
                    <tbody>
");


            
            #line 110 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                     foreach (var job in succeededJobs)
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <tr class=\"js-jobs-list-row ");


            
            #line 112 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                                Write(job.Value != null && !job.Value.InSucceededState ? "obsolete-data" : null);

            
            #line default
            #line hidden
WriteLiteral(" ");


            
            #line 112 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                                                                                                             Write(job.Value != null && job.Value.InSucceededState ? "hover" : null);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n                            <td>\r\n");


            
            #line 114 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                 if (job.Value != null && job.Value.InSucceededState)
                                {

            
            #line default
            #line hidden
WriteLiteral("                                    <input type=\"checkbox\" class=\"js-jobs-list-ch" +
"eckbox\" name=\"jobs[]\" value=\"");


            
            #line 116 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                                                                                         Write(job.Key);

            
            #line default
            #line hidden
WriteLiteral("\"/>\r\n");


            
            #line 117 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                }

            
            #line default
            #line hidden
WriteLiteral("                            </td>\r\n                            <td class=\"min-wid" +
"th\">\r\n                                ");


            
            #line 120 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                           Write(Html.JobIdLink(job.Key));

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 121 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                 if (job.Value != null && !job.Value.InSucceededState)
                                {

            
            #line default
            #line hidden
WriteLiteral("                                    <span title=\"Job\'s state has been changed whi" +
"le fetching data.\" class=\"glyphicon glyphicon-question-sign\"></span>\r\n");


            
            #line 124 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                }

            
            #line default
            #line hidden
WriteLiteral("                            </td>\r\n\r\n");


            
            #line 127 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                             if (job.Value == null)
                            {

            
            #line default
            #line hidden
WriteLiteral("                                <td colspan=\"3\">\r\n                               " +
"     <em>Job has expired.</em>\r\n                                </td>\r\n");


            
            #line 132 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                            }
                            else
                            {

            
            #line default
            #line hidden
WriteLiteral("                                <td>\r\n                                    ");


            
            #line 136 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                               Write(Html.JobNameLink(job.Key, job.Value.Job));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n");



WriteLiteral("                                <td class=\"min-width align-right\">\r\n");


            
            #line 139 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                     if (job.Value.TotalDuration.HasValue)
                                    {
                                        
            
            #line default
            #line hidden
            
            #line 141 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                   Write(Html.ToHumanDuration(TimeSpan.FromMilliseconds(job.Value.TotalDuration.Value), false));

            
            #line default
            #line hidden
            
            #line 141 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                                                                                                              
                                    }

            
            #line default
            #line hidden
WriteLiteral("                                </td>\r\n");



WriteLiteral("                                <td class=\"min-width align-right\">\r\n");


            
            #line 145 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                     if (job.Value.SucceededAt.HasValue)
                                    {
                                        
            
            #line default
            #line hidden
            
            #line 147 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                   Write(Html.Raw(job.Value.SucceededAt.Value.ToString("dd/MM/yyyy HH:mm")));

            
            #line default
            #line hidden
            
            #line 147 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                                                                                                           ;
                                    }

            
            #line default
            #line hidden
WriteLiteral("                                </td>\r\n");


            
            #line 150 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                            }

            
            #line default
            #line hidden
WriteLiteral("                        </tr>\r\n");


            
            #line 152 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                    </tbody>\r\n                </table>\r\n\r\n                ");


            
            #line 156 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
           Write(Html.Paginator(pager));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n");


            
            #line 158 "..\..\Dashboard\Pages\SucceededJobs.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n</div>");


        }
    }
}
#pragma warning restore 1591
