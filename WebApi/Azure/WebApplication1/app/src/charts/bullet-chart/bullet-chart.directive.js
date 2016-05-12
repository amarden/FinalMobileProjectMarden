angular.module('ehrDashboard')
    .directive("bulletChart", function ($timeout) {
        return {
            restrict: 'E',
            scope: {
                data: '=',
                outerProp: '=',
                innerProp: '=',
                groupBy: '=',
                filter: '='
            },
            link: function (scope, elem) {
                var bulletChart = function () {
                    // directive variables
                    var svg, yScale, chart, xScale;
                    var outerProp = "staffed";
                    var innerProp = "charge";
                    var group = "project";
                    var drawn = false;
                    var monthsBack = 0;

                    var setTitle = function () {
                        var myDate = new moment().subtract("month", monthsBack).format('MMMM');
                        chart.select(".title")
                            .text(myDate + " Project Variance");
                    };

                    var createChart = function () {
                        // Set up the left side margin:
                        var margin = { top: 15, right: 50, bottom: 20, left: 40 };
                        var w = elem[0].offsetWidth;
                        var h = elem[0].offsetHeight;
                        svg = d3.select(elem[0])
                            .append("svg")
                            .attr("height", h)
                            .attr("width", w)
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

                        chart = svg.append("g")
                            .attr("transform", "translate(" + margin.left + "," + (0) + ")");

                        chart.append("text")
                            .attr("class", "backward to-show")
                            .attr("dx", -10)
                            .attr("dy", margin.top - 5)
                            .style("font-size", 10)
                            .attr("text-anchor", "middle")
                            .text('<')
                            .style("cursor", "pointer")
                            .on("click", function () {
                                scope.filter('backward');
                                monthsBack++;
                                scope.$apply();
                            })
                            .style("opacity", 0);

                        chart.append("text")
                           .attr("class", "forward to-show")
                           .attr("dx", w - margin.right - 5)
                           .attr("dy", margin.top - 5)
                           .style("font-size", 10)
                           .attr("text-anchor", "middle")
                           .text('>')
                           .style("cursor", "pointer")
                           .on("click", function () {
                               scope.filter('forward');
                               monthsBack--;
                               scope.$apply();
                           })
                           .style("opacity", 0);

                        chart.append("text")
                            .attr("class", "title to-show")
                            .attr("dx", (w - margin.right) / 2)
                            .attr("dy", margin.top - 5)
                            .style("font-size", 10)
                            .attr("text-anchor", "middle")
                            .style("opacity", 0);

                        // Set up the scales:
                        xScale = d3.scale.linear()
                            .range([0, w - margin.right]);

                        yScale = d3.scale.ordinal()
                            .rangeBands([margin.top, h - margin.bottom], 0.1);

                        //Set up axes:
                        chart.append("g")
                            .attr("class", "x axis to-show")
                            .attr("transform", "translate(0, " + (h - margin.bottom - 5) + ")")
                            .style("opacity", 0)
                            .append("text")
                            .style("font-size", "8")
                            .style("text-anchor", "middle")
                            .attr("dx", (w - margin.right) / 2)
                            .attr("dy", 25)
                            .text("Hours");
                        drawn = true;
                        updateChart(scope.data);
                    };

                    var updateChart = function (data) {
                        setTitle();
                        chart.select(".forward")
                            .style("display", function () { return monthsBack >= 1 ? "block" : "none"; });
                        chart.select(".backward")
                            .style("display", function () { return monthsBack >= 3 ? "none" : "block"; });

                        //Set up the SVG:
                        var maxX = d3.max(data, function (d) { return Math.max(d[innerProp], d[outerProp]); }) * 1.10;
                        xScale.domain([0, maxX]);

                        var xAxis = d3.svg.axis()
                           .scale(xScale)
                           .ticks(5)
                           .orient("bottom");

                        chart.select('.x')
                            .call(xAxis);

                        yScale.domain(data.map(function (d) { return d[group]; }));

                        var innerBarHeight = yScale.rangeBand() * 0.6;
                        // Data binding:
                        var barsCont = chart.selectAll("g.bars")
                            .data(data, function (d) { return d[group]; });

                        barsCont.transition().duration(500)
                            .attr("transform", function (d) { return "translate(0, " + yScale(d[group]) + ")"; });

                        barsCont.select("rect.outer-bar")
                            .attr("height", yScale.rangeBand())
                            .transition().duration(500)
                            .attr("width", function (d) { return xScale(d[outerProp]); });

                        barsCont.select("rect.inner-bar")
                           .attr("height", innerBarHeight)
                           .attr("y", (yScale.rangeBand() - innerBarHeight) / 2)
                           .transition().duration(500)
                           .attr("width", function (d) { return xScale(d[innerProp]); });

                        barsCont.select("text")
                          .attr("dy", yScale.rangeBand() / 2);
                        // Create container per observation:;
                        var barEnter = barsCont.enter().append("g")
                            .attr("class", "bars")
                            .attr("transform", function (d) { return "translate(0, " + yScale(d[group]) + ")"; });

                        // Add Outer bar
                        barEnter.append("rect")
                            .attr("class", "outer-bar")
                            .attr("height", yScale.rangeBand())
                            .attr("width", function (d) { return xScale(d[outerProp]); })
                            .attr("fill", "black")
                            .style("opacity", 0.3);

                        //Add InnerBar
                        barEnter.append("rect")
                           .attr("class", "inner-bar")
                           .attr("height", innerBarHeight)
                           .attr("y", (yScale.rangeBand() - innerBarHeight) / 2)
                           .attr("width", function (d) { return xScale(d[innerProp]); })
                           .attr("fill", "steelblue")
                           .style("opacity", 0.8);

                        // Add group name
                        barEnter.append("text")
                            .attr("class", "name-month")
                            .attr("text-anchor", "end")
                            .attr("dominant-baseline", "central")
                            .attr("font-size", "10")
                            .attr("dx", -1)
                            .attr("dy", yScale.rangeBand() / 2)
                            .text(function (d) { return d[group]; });

                        barsCont.exit().remove();
                    };

                    scope.$watch("data", function () {
                        if (scope.data && !drawn) {
                            createChart(scope.data);
                        } else {
                            updateChart(scope.data);
                        }
                    }, true);
                };
                $timeout(bulletChart, 0);
            }
        };
    });
