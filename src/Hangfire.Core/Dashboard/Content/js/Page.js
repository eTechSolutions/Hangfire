(function (hangFire) {
    hangFire.Page = (function () {
        var url;
        function Page(config) {
            this._metrics = new Hangfire.Metrics();

            var self = this;
            this._poller = new Hangfire.StatisticsPoller(
                function () { return self._metrics.getNames(); },
                config.pollUrl,
                config.pollInterval);

            this._initialize();
            this._createGraphs();
            this._poller.start();
        }

        Page.prototype._createGraphs = function() {
            this.realtimeGraph = this._createRealtimeGraph('realtimeGraph');
            this.historyGraph = this._createHistoryGraph('historyGraph');
            
            var debounce = function (fn, timeout) {
                var timeoutId = -1;
                return function() {
                    if (timeoutId > -1) {
                        window.clearTimeout(timeoutId);
                    }
                    timeoutId = window.setTimeout(fn, timeout);
                };
            };

            var self = this;
            window.onresize = debounce(function () {
                $('#realtimeGraph').html('');
                $('#historyGraph').html('');

                self._createGraphs();
            }, 125);
        };

        Page.prototype._createRealtimeGraph = function(elementId) {
            var realtimeElement = document.getElementById(elementId);
            var succeeded = parseInt($(realtimeElement).data('succeeded'));
            var failed = parseInt($(realtimeElement).data('failed'));

            if (realtimeElement) {
                var realtimeGraph = new Hangfire.RealtimeGraph(realtimeElement, succeeded, failed);

                this._poller.addListener(function (data) {
                    realtimeGraph.appendHistory(data);
                });

                return realtimeGraph;
            }

            return null;
        };

        Page.prototype._createHistoryGraph = function(elementId) {
            var historyElement = document.getElementById(elementId);
            if (historyElement) {
                var createSeries = function (obj) {
                    var series = [];
                    for (var date in obj) {
                        if (obj.hasOwnProperty(date)) {
                            var value = obj[date];
                            var point = { x: Date.parse(date) / 1000, y: value };
                            series.unshift(point);
                        }
                    }
                    return series;
                };

                var succeeded = createSeries($(historyElement).data("succeeded"));
                var failed = createSeries($(historyElement).data("failed"));

                return new Hangfire.HistoryGraph(historyElement, succeeded, failed);
            }

            return null;
        };

        Page.prototype._initialize = function() {
            var updateRelativeDates = function () {
                $('*[data-moment]').each(function () {
                    var $this = $(this);
                    var timestamp = $this.data('moment');

                    if (timestamp) {
                        var time = moment(timestamp, 'X');
                        $this.html(time.fromNow())
                            .attr('title', time.format('llll'))
                            .attr('data-container', 'body');
                    }
                });

                $('*[data-moment-title]').each(function () {
                    var $this = $(this);
                    var timestamp = $this.data('moment-title');

                    if (timestamp) {
                        var time = moment(timestamp, 'X');
                        $this.prop('title', time.format('llll'))
                            .attr('data-container', 'body');
                    }
                });
            };

            updateRelativeDates();
            setInterval(updateRelativeDates, 30 * 1000);

            $('*[title]').tooltip();

            var self = this;
            $('*[data-metric]').each(function () {
                var name = $(this).data('metric');
                self._metrics.addElement(name, this);
            });

            this._poller.addListener(function (metrics) {
                for (var name in metrics) {
                    var elements = self._metrics.getElements(name);
                    for (var i = 0; i < elements.length; i++) {
                        var metric = metrics[name];
                        var metricClass = metric ? "metric-" + metric.style : "metric-null";
                        var highlighted = metric && metric.highlighted ? "highlighted" : null;
                        var value = metric ? metric.value : null;

                        $(elements[i])
                            .text(value)
                            .closest('.metric')
                            .removeClass()
                            .addClass(["metric", metricClass, highlighted].join(' '));
                    }
                }
            });

            $(document).on('click', '*[data-ajax]', function (e) {
                var $this = $(this);
                var confirmText = $this.data('confirm');

                if (!confirmText || confirm(confirmText)) {
                    var loadingDelay = setTimeout(function() {
                        $this.button('loading');
                    }, 100);

                    $.post($this.data('ajax'), function() {
                        clearTimeout(loadingDelay);
                        $this.button('reset');
                        window.location.reload();
                    });
                }

                e.preventDefault();
            });

            $(document).on('click', '.expander', function (e) {
                var $expander = $(this),
                    $expandable = $expander.closest('tr').next().find('.expandable');

                if (!$expandable.is(':visible')) {
                    $expander.text('Less details...');
                }

				$expandable.slideToggle(
					150, 
					function() {
					    if (!$expandable.is(':visible')) {
					        $expander.text('More details...');
					    }
					});
                e.preventDefault();
            });

            $('.js-jobs-list').each(function () {
                var container = this;


                var selectRow = function(row, isSelected) {
                    var $checkbox = $('.js-jobs-list-checkbox', row);
                    if ($checkbox.length > 0) {
                        $checkbox.prop('checked', isSelected);
                        $(row).toggleClass('highlight', isSelected);
                    }
                };

                var toggleRowSelection = function(row) {
                    var $checkbox = $('.js-jobs-list-checkbox', row);
                    if ($checkbox.length > 0) {
                        var isSelected = $checkbox.is(':checked');
                        selectRow(row, !isSelected);
                    }
                };

                var setListState = function (state) {
                    $('.js-jobs-list-select-all', container)
                        .prop('checked', state === 'all-selected')
                        .prop('indeterminate', state === 'some-selected');
                    
                    $('.js-jobs-list-command', container)
                        .prop('disabled', state === 'none-selected');
                };

                var updateListState = function() {
                    var selectedRows = $('.js-jobs-list-checkbox', container).map(function() {
                        return $(this).prop('checked');
                    }).get();

                    var state = 'none-selected';

                    if (selectedRows.length > 0) {
                        state = 'some-selected';

                        if ($.inArray(false, selectedRows) === -1) {
                            state = 'all-selected';
                        } else if ($.inArray(true, selectedRows) === -1) {
                            state = 'none-selected';
                        }
                    }

                    setListState(state);
                };

                $(this).on('click', '.js-jobs-list-checkbox', function(e) {
                    selectRow(
                        $(this).closest('.js-jobs-list-row').first(),
                        $(this).is(':checked'));

                    updateListState();

                    e.stopPropagation();
                });

                $(this).on('click', '.js-jobs-list-row', function (e) {
                    if ($(e.target).is('a')) return;

                    toggleRowSelection(this);
                    updateListState();
                });

                $(this).on('click', '.js-jobs-list-select-all', function() {
                    var selectRows = $(this).is(':checked');

                    $('.js-jobs-list-row', container).each(function() {
                        selectRow(this, selectRows);
                    });

                    updateListState();
                });

                $(this).on('click', '.js-jobs-list-command', function(e) {
                    var $this = $(this);
                    var confirmText = $this.data('confirm');

                    var jobs = $("input[name='jobs[]']:checked", container).map(function() {
                        return $(this).val();
                    }).get();

                    if (!confirmText || confirm(confirmText)) {
                        var loadingDelay = setTimeout(function () {
                            $this.button('loading');
                        }, 100);

                        $.post($this.data('url'), { 'jobs[]': jobs }, function () {
                            clearTimeout(loadingDelay);
                            $this.button('reset');
                            window.location.reload();
                        });
                    }
                    e.preventDefault();
                });


                $(this).on('click', '.js-jobs-filter-command', function (e) {                    
                    url = document.location.search;
                    var filterValueString = $("#filterValueString").val();
                    var filterStartDate = $("#startDate").datepick({ dateFormat: 'dd-mm-yyyy' }).val();
                    var filterEndDate = $("#endDate").datepick({ dateFormat: 'dd-mm-yyyy' }).val();
                    var addedFilterString = prepFilterStringParameter(filterValueString);                        
                    var addedDateFilterStrings = prepFilterDateParameters(filterStartDate, filterEndDate);
                    if (addedDateFilterStrings || addedFilterString) {                                  
                        redirectToFirstPage();                        
                    }                    
                    document.location.search = url;                                      
                })
                
                var prepFilterStringParameter = function (filterString) {                    
                    var result = false;
                    if (url == '' && filterString != '') {                        
                        url = '?' + "filterString=" + filterString;                       
                        result = true;
                    } else {
                        var parameters = url.substr(1).split('&');                        
                        var elements;
                        var i = 0;
                        for(i=0;i < parameters.length;i++){
                            elements = parameters[i].split('=');
                            if (elements[0] == "filterString" && filterString != '' ) {
                                elements[1] = filterString;
                                parameters[i] = elements.join('=');                                
                                result = true;
                                break;
                            } else if (elements[0] == "filterString" && filterString == '') {
                                parameters.splice(i, 1);                                
                                result = false;
                                break;
                            }
                        }
                        if ( i >= parameters.length && filterString != '' ) {
                            parameters[parameters.length] = ["filterString", filterString].join('=');  
                            result = true;
                        }                        
                        url = '?' + parameters.join('&');                        
                    }                   
                    return result;                    
                }

                var prepFilterDateParameters = function (startDate, endDate) {
                    var sDate = startDate.split('/'),
                    eDate = endDate.split('/');
                    var sTime = new Date(sDate[2],sDate[1],sDate[0]);
                    var eTime = new Date(eDate[2], eDate[1], eDate[0]);
                    var checked = $("#filterOnDate").is(':checked');
                    if ((sTime - eTime) / (1000 * 24 * 60 * 60) <= 0 && checked) {                        
                        if (url == '') {                            
                            url = '?' + "startDate=" + sDate.join('-') + '&' + "endDate=" + eDate.join('-');                            
                        } else {                            
                            var parameters = url.substr(1).split('&');
                            var element;
                            var foundStart = false;
                            var foundEnd = false;
                            for (var i = 0; i < parameters.length; i++) {                                
                                element = parameters[i].split('=');
                                if (element[0] == "startDate") {
                                    element[1] = sDate.join('-');
                                    parameters[i] = element.join('=');
                                    foundStart = true;                                    
                                } else if (element[0] == "endDate") {
                                    element[1] = eDate.join('-');
                                    parameters[i] = element.join('=');
                                    foundEnd = true;
                                }                                                                
                            }
                            if (!foundStart) {
                                parameters[parameters.length] = ["startDate", sDate.join('-')].join('=');
                            }
                            if (!foundEnd) {
                                parameters[parameters.length] = ["endDate", eDate.join('-')].join('=');
                            }
                            url = '?' + parameters.join('&');                           
                        }                       
                        return true;
                    }
                    else if ( url.indexOf("startDate") > -1 || url.indexOf("endDate") > -1 ) {
                        if (url != '') {
                            var parameters = url.substr(1).split('&');
                            var element;
                            var i = 0;
                            do{
                                element = parameters[i].split('=');
                                if (element[0] == "startDate" || element[0] == "endDate") {
                                    parameters.splice(i, 1);                                    
                                } else {
                                    i++;
                                }                                
                            } while (i < parameters.length)                           
                            url = '?' + parameters.join('&');
                        }     
                    }
                    return false;               
                }

                var redirectToFirstPage = function () {                    
                    if (url.indexOf("from") > -1 ) {
                        var parameters = url.substr(1).split('&');
                        var element;
                        for (var i = 0; i < parameters.length; i++) {
                            element = parameters[i].split('=');
                            if (element[0] == "from") {
                                element[1] = 0;
                                parameters[i] = element.join('=');
                                break;
                            }
                        }                        
                        url = '?' + parameters.join('&');                       
                    }
                }
                                
                $(this).on('click', '.js-jobs-filtertext-clear', function (e) {                    
                    $("#filterValueString").val('');
                })

                $(this).on('click', '.js-jobs-filterOnDate-checked', function (e) {  
                    var checked = $('input[type=checkbox]').is(':checked');
                    if (checked) {                        
                        document.getElementById("startDate").hidden = "";
                        document.getElementById("endDate").hidden = "";         
                    }
                    else {                       
                        document.getElementById("startDate").hidden = "hidden";
                        document.getElementById("endDate").hidden = "hidden";                                               
                    }
                })

                $(".dateselector-start").datepick();
                $(".dateselector-end").datepick();
                updateListState();
            });            
        };

        return Page;
    })();
})(window.Hangfire = window.Hangfire || {});

$(function () {
    Hangfire.page = new Hangfire.Page(Hangfire.config);
});
