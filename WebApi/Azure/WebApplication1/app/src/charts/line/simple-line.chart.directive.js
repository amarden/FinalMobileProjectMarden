angular.module("ehrDashboard")
    .directive("simpleLineChart", function ($timeout, LineOptions) {
    return {
        restrict: 'E',
        scope: {
            data: '=',
            options: '=',
            date: '=',
            unit: '='
        },
        link: function (scope, elem) {
            var lineChart = function () {
                var config = LineOptions.generateOptions(scope.options);
                var w, h;
                var unit = scope.unit;
                var date = scope.date;
                var radius = config.radius;
                var lineTip, svg, xScale, yScale;
                //var formatMonth = d3.time.format("%Y-%m-%d");
                var formatMonthLabel = d3.time.format("%b-%y");

                var createChart = function (data) {
                    w = config.width ? config.width : elem[0].offsetWidth - 20;
                    h = config.height ? config.height : elem[0].offsetHeight;
                    svg = d3.select(elem[0]).append("svg")
                        .attr("class", "svg-simple-line")
                        .attr("width", w).attr("height", h)
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

                    var chart = svg.append("g")
                        .attr("class", "chart")
                        .attr("transform", "translate(0,0)");

                    chart.append("text")
                        .attr("class", "title to-show")
                        .attr("dx", w / 2)
                        .attr("dy", 10)
                        .style("font-size", 10)
                        .attr("text-anchor", "middle")
                        .text(config.title)
                        .style("opacity", 0);

                    chart.append("g")
                        .attr("class", "x-axis-time to-show")
                        .attr("transform", "translate(0," + (h - 25) + ")")
                        .style("opacity", 0);

                    chart.append("g")
                      .attr("class", "y-axis-time")
                      .attr("transform", "translate(80,0)");

                    if (config.tooltip) {
                        createToolTip();
                    }

                    data = data.map(function (d) {
                        return {
                            unit: d[unit],
                            //date: d[date].substring(0, 10),
                            realDate: d[date]                        };
                    });

                    data = data.sort(function (a, b) {
                        return +a.realDate - b.realDate;
                    });

                    // Set up scales
                    var minVal = 0;
                    var maxVal = d3.max(data, function (d) { return d.unit; });

                    // Give 10% buffers on both sides
                    maxVal = maxVal + (maxVal * 0.05);
                    yScale = d3.scale.linear()
                        .domain([minVal, config.yMax])
                        .range([(h - 10), 10]);
                    xScale = d3.time.scale()
                        .range([15, w-20])
                        .domain(d3.extent(data, function (d) { return d.realDate; }));

                    updateChart(data);
                };

                var createLine = function() {
                    var chart = svg.select("g.chart");

                    chart.append("line")
                        .attr("x1", 15)
                        .attr("x2", w - 20)
                        .attr("y1", yScale(config.straightLine))
                        .attr("y2", yScale(config.straightLine))
                        .style("stroke", "black")
                        .style("opacity", 0.4);

                    chart.append("text")
                        .attr("class", "to-show")
                        .attr("dx", (w - 20))
                        .attr("dy", yScale(config.straightLine) + 2)
                        .attr("font-size", 8)
                        .style("opacity", 0)
                        .text(config.straightLine + "%");
                };

                var createToolTip = function () {
                    lineTip = d3.tip().attr('class', 'd3-tip')
                        .offset(function () {
                            return [-8, 0];
                        })
                        .direction('n')
                        .style("text-align", "center")
                        .html(function (d) {
                            return Math.round(d.unit * 100) / 100;
                        });
                    svg.call(lineTip);
                };

                var updateChart = function (data) {
                    createLine();
                    var chart = svg.select("g.chart");

                    var xAxis = d3.svg.axis()
                        .scale(xScale)
                        .tickFormat(formatMonthLabel)
                        .tickPadding(6)
                        .ticks(d3.time.month, config.xAxisFreq)
                        .orient("bottom");

                    var yAxis = d3.svg.axis()
                        .scale(yScale)
                        .ticks(6)
                        .orient("left");

                    var line = d3.svg.line()
                        .x(function (d) { return xScale(d.realDate); })
                        .y(function (d) { return yScale(d.unit); });

                    chart.append("path")
                        .datum(data)
                        .attr("class", "lines")
                        .style("stroke", config.color)
                        .transition()
                        .ease("linear")
                        .duration(1000)
                        .attr("d", function (d) { return line(d); });

                    // Axes
                    svg.select("g.x-axis-time")
                        .call(xAxis);

                    //// Axes
                    //d3.select("g.y-axis-time")
                    //    .call(yAxis);

                    //Circles
                    var circles = chart.selectAll("circle")
                        .data(data);

                    circles.enter().append("circle")
                        .style("fill", config.color)
                        .attr("r", radius)
                        .on('mouseover', function (d) {
                            d3.select(this).transition().duration(500)
                                .attr("r", (radius + (radius * 0.9)));
                            lineTip.show(d);
                        })
                        .on('mouseout', function (d) {
                            d3.select(this).transition().duration(500).attr("r", radius);
                            lineTip.hide(d);
                        });

                    circles.transition().duration(1000)
                        .ease("bounce")
                        .attr("cx", function (d) { return xScale(d.realDate); })
                        .attr("cy", function (d) { return yScale(d.unit); });

                    circles.exit().remove();
                };

                scope.$watch('data', function () {
                    if (scope.data) {
                        createChart(scope.data);
                    }
                }, true);
            };
            $timeout(lineChart, 0);
        }
    };
});
