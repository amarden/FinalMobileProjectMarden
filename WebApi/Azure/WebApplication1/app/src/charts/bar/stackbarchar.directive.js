angular.module('ehrDashboard')
    .directive("stackBarChart", function ($timeout, ChartOptions) {
        return {
            restrict: 'E',
            scope: {
                data: '=',
                options: '=',
                date: '=',
                group: '=',
                unit: '='
            },
            link: function (scope, elem) {
                var stackBar = function () {
                    var svg, chart, width, height, yAxis;
                    var config = ChartOptions.generateOptions(scope.options);
                    var color = config.colorScale;
                    var toolTip;
                    //var formatDate = config.dateFormat;
                    var unit = scope.unit;
                    var date = scope.date;
                    var group = scope.group;
                    var stack = d3.layout.stack()
                       .values(function (d) { return d.values; })
                       .x(function (d) { return d.realDate; })
                       .y(function (d) { return d.unit; });

                    var margin = config.margin;

                    var drawYAxis = function () {
                        chart.append("g")
                            .attr("class", "y axis")
                            .attr("transform", "translate(50,0)")
                            .call(yAxis)
                          .append("text")
                            .attr("transform", "rotate(-90)")
                            .attr("y", -45)
                            .attr("dx", -(height - 100) / 2)
                            .attr("dy", ".71em")
                            .style("text-anchor", "middle")
                            .text(config.yScaleLabel);
                    };

                    var drawLegend = function (data) {
                        //Legend
                        var onlyValues = data.filter(function (d) { return d.unit !== 0; });
                        var legend = chart.selectAll(".legend")
                            .data(_.uniq(_.pluck(onlyValues, 'group')))
                          .enter().append("g")
                            .attr("class", "legend")
                            .attr("transform", function (d, i) { return "translate(0," + i * 20 + ")"; });

                        legend.append("rect")
                            .attr("x", width - 72)
                            .attr("width", 18)
                            .attr("height", 18)
                            .style("fill", color);

                        legend.append("text")
                            .attr("x", width - 96)
                            .attr("y", 9)
                            .attr("dy", ".35em")
                            .style("text-anchor", "end")
                            .text(function (d) { return d; });
                    };

                    var createToolTip = function () {
                        toolTip = d3.tip().attr('class', 'd3-tip')
                            .offset(function () {
                                return [config.toolTipAdjustTop, 0];
                            })
                            .direction('e')
                            .style("text-align", "center")
                            .html(function (d) {
                                return d.group + "<br /> " + d.unit + " " + config.toolTipUnit;
                            });
                        svg.call(toolTip);
                    };

                    var createChart = function (data) {
                        data = data.map(function (d) {
                            return {
                                unit: d[unit],
                                date: d[date],
                                group: d[group],
                                realDate: new moment(d[date])._d
                            };
                        });

                        var dates = _.uniq(data.map(function (d) { return d.date; }));
                        var categories = _.uniq(data.map(function (d) { return d.group; }));

                        dates.forEach(function (date) {
                            categories.forEach(function (cat) {
                                var match = _.find(data, function (d) { return d.date === date && d.group === cat; });
                                if (!match) {
                                    var dummy = { unit: 0, group: cat, realDate: new moment(date)._d, date: date };
                                    data.push(dummy);
                                }
                            });
                        });

                        data = _.sortBy(data, function (d) { return d.realDate; });
                        data = _.sortBy(data, function (d) { return d.group; });

                        var groups = d3.nest()
                            .key(function (d) { return d.group; }).entries(data);

                        var days = stack(groups);
                        width = elem[0].offsetWidth;
                        height = config.height ? config.height : elem[0].offsetHeight;
                        var chartHeight = height - margin.bottom;

                        svg = d3.select(elem[0]).append("svg")
                            .attr("class", "svg-chart")
                            .attr("width", width).attr("height", height);

                        chart = svg.append("g")
                            .attr("class", "chart")
                            .attr("transform", "translate(0,0)")
                            .on("mouseover", function () {
                                d3.select(this).selectAll(".to-show")
                                       .transition().duration(500)
                                       .style("opacity", 1);
                            })
                            .on("mouseout", function () {
                                d3.select(this).selectAll(".to-show")
                                    .transition().duration(500)
                                    .style("opacity", 0);
                            });

                        chart.append("text")
                            .attr("class", "title to-show")
                            .attr("dx", width / 2)
                            .attr("dy", margin.top - 10)
                            .style("font-size", 10)
                            .attr("text-anchor", "middle")
                            .text(config.titleText)
                            .style("opacity", 0);

                        var xScale = d3.scale.ordinal()
                            .rangeRoundBands([margin.left, width - margin.right], 0.08)
                            .domain(_.uniq(data.map(function (d) { return d.realDate; })));

                        var min = typeof (config.min) === 'number' ? config.min :
                            d3.min(data, function (d) { return d.unit; });

                        var totals = _.map(_.groupBy(data, 'date'),
                            function (data) { return _.reduce(data, function (m, x) { return m + x.unit; }, 0); });

                        var max = typeof (config.max) === 'number' ? config.max :
                            d3.max(totals, function (d) { return d; });

                        var yScale = d3.scale.linear()
                            .range([chartHeight, margin.top])
                            .domain([min, max]);

                        var xAxis = d3.svg.axis()
                            .scale(xScale)
                            .tickFormat(config.dateAxisFormat)
                            .orient("bottom");

                        yAxis = d3.svg.axis()
                            .scale(yScale)
                            .orient("left");

                        chart.append("g")
                            .attr("class", "x axis to-show")
                            .style("opacity", 0)
                            .attr("transform", "translate(0," + (height - margin.bottom - 5) + ")")
                            .call(xAxis);

                        // Add a group for each cause.
                        var barCont = chart.selectAll("g.day")
                            .data(days)
                          .enter().append("svg:g")
                            .attr("class", "day")
                            .style("fill", function (d) { return color(d.key); })
                            .style("stroke", function (d) { return d3.rgb(color(d.key)).darker(); });

                        // Add a rect for each date.
                        barCont.selectAll("rect")
                            .data(function (d) { return d.values; })
                          .enter().append("svg:rect")
                            .attr("x", function (d) { return xScale(d.realDate); })
                            .attr("y", function (d) { return yScale(d.y) - ((chartHeight) - yScale(d.y0)); })
                            .attr("height", function (d) { return (chartHeight) - yScale(d.y); })
                            .attr("width", xScale.rangeBand())
                            .on('mouseover', function (d) {
                                toolTip.show(d);
                            })
                            .on('mouseout', function (d) {
                                toolTip.hide(d);
                            });

                        createToolTip();
                        if (config.drawYAxis) {
                            drawYAxis(data);
                        }
                        if (config.drawLegend) {
                            drawLegend(data);
                        }
                    };

                    scope.$watch('data', function () {
                        if (scope.data) {
                            setTimeout(function () { createChart(scope.data); }, 250);
                        }
                    }, true);
                };
                $timeout(stackBar, 0);
            }
        };
    });
