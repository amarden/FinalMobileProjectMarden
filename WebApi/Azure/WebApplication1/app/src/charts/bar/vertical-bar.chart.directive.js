angular.module("ehrDashboard")
    .directive("vBarChart", function (BarOptions) {
        return {
            restrict: 'E',
            scope: {
                data: '=',
                options: '=',
                group: '=',
                unit: '=',
            },
            link: function (scope, elem) {
                var config = BarOptions.generateOptions(scope.options);
                var w, h;
                var y = scope.group || 'group';
                var x = scope.unit || 'unit';
                var drawn = false;
                var barTip;
                var svg;

                var createChart = function () {
                    w = config.width ? config.width : elem[0].offsetWidth;
                    h = config.height ? config.height : elem[0].offsetHeight;

                    svg = d3.select(elem[0]).append("svg")
                        .attr("width", w).attr("height", h);

                    svg.append("text")
                       .attr("class", "title")
                       .attr("dx", w / 2)
                       .attr("dy", 10)
                       .style("font-size", 10)
                       .attr("text-anchor", "middle")
                       .text(config.chartTitle);

                    svg.append("g")
                        .attr("class", "chart")
                        .attr("transform", "translate(" + config.margin.left + "," + config.margin.top + ")");

                    svg.select("g.chart").append("g")
                        .attr("class", "x-axis")
                        .attr("transform", "translate(0,5)");

                    if (config.tooltip) {
                        createToolTip();
                    }

                    updateChart(scope.data);
                };

                var createToolTip = function () {
                    barTip = d3.tip().attr('class', 'd3-tip')
                        .offset(function () {
                            return [0, 2];
                        })
                        .direction('e')
                        .style("text-align", "center")
                        .html(function (d) {
                            return Math.round(d[x] * 100) / 100;
                        });
                    svg.call(barTip);
                };

                var updateChart = function (data) {
                    data.sort(function (a, b) {
                        return a[y] < b[y] ? 1 : -1;
                    });

                    var yScaleHeight = Math.max(h - config.margin.top, data.length * 13);

                    if (data.length * 13 > h - config.margin.top) {
                        svg.attr("height", (data.length * 13 + config.margin.top));
                    }
                    else {
                        svg.attr("height", h);
                    }
                    d3.select(elem[0]).style("height", h + "px");

                    var xMax = config.maxX || d3.max(data.filter(function (d) { return d[x] !== Infinity; }), function (d) { return d[x]; });
                    var xScale = d3.scale.linear()
                        .domain([0, xMax])
                        .range([0, (w - config.margin.right - config.margin.left)]);

                    var yScale = d3.scale.ordinal()
                        .domain(data.map(function (d) { return d[y]; }))
                        .rangeBands([10, yScaleHeight - config.margin.bottom], 0.05);

                    var barHeight = yScale.rangeBand();

                    var xAxis = d3.svg.axis()
                       .scale(xScale)
                       .orient("top");

                    if (config.tickNumbers) {
                        xAxis.ticks(5);
                    }

                    //svg.select("g.x-axis")
                    //    .transition().duration(500)
                    //    .call(xAxis);

                    console.log(data, y, x);

                    var barContainer = svg.select("g.chart").selectAll("g.bars")
                        .data(data, function (d) { return d[y]; });

                    barContainer.exit()
                        .transition().duration(500)
                        .delay(function (d, i) { return i * 20; })
                        .attr("transform", "translate(0," + (h + 200) + ")")
                        .remove();

                    var barEnter = barContainer.enter().append("g")
                        .attr("class", "bars")
                        .attr("transform", function (d) {
                            return "translate(0," + yScale(d[y]) + ")";
                        })
                        .style("opacity", 0);

                    barEnter.append("rect")
                        .attr("class", "bar")
                        .attr("y", 0)
                        .attr("fill", function (d) {
                            return config.colorBy === 'none' ? config.color :
                                color(d[config.colorBy]);
                        })
                        .on("mouseover", barTip.show)
                        .on("mouseout", barTip.hide);

                    barEnter.append("text")
                        .attr("class", "bar-label")
                        .attr("text-anchor", "end")
                        .attr("font-size", "12")
                        .attr("dx", -1)
                        .attr("dy", ".35em");

                    barContainer.select("rect")
                        .transition().duration(500)
                        .attr("width", function (d) {
                            return d[x] === Infinity ? 0 : xScale(d[x]);
                        })
                        .attr("height", barHeight);

                    barContainer.select("text.bar-label")
                        .transition().duration(500)
                        .attr("y", barHeight / 2)
                        .text(function (d) {
                            return d[y];
                        });

                    barContainer.transition().duration(1000)
                        .style("opacity", 1)
                        .attr("transform", function (d) { return "translate(0," + yScale(d[y]) + ")"; });
                };

                scope.$watch('data', function () {
                    if (scope.data) {
                        if (!drawn) {
                            setTimeout(function () { createChart(); }, 250);
                        } else {
                            updateChart(scope.data);
                        }
                        drawn = true;
                    }
                }, true);
            }
        };
    });
