angular.module('ehrDashboard')
    .directive("lineChart", function (ChartOptions) {
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
            var config = ChartOptions.generateOptions(scope.options);
            var w, h;
            var unit = scope.unit;
            var date = scope.date;
            var group = scope.group;
            var drawn = false;
            var color = config.colorScale;
            var lineTip;
            var formatDate = d3.time.format("%m/%d/%Y");

            var createChart = function () {
                w = elem[0].offsetWidth;
                h = elem[0].offsetHeight;
                console.log(w, h);

                var svg = d3.select(elem[0]).append("svg")
                    .attr("class", "svg-chart")
                    .attr("width", w).attr("height", h);

                var chart = svg.append("g")
                    .attr("class", "chart")
                    .attr("transform", "translate(0,0)");

                chart.append("g")
                  .attr("class", "x-axis")
                  .attr("transform", "translate(0," + (h - 100) + ")");

                chart.append("g")
                  .attr("class", "y-axis")
                  .attr("transform", "translate(80,0)")
                    .append("text")
                    .attr("transform", "rotate(-90)")
                    .attr("y", -40)
                    .attr("dx", -(h - 100) / 2)
                    .attr("dy", ".71em")
                    .style("text-anchor", "middle")
                    .text(config.yScaleLabel);

                //if (config.tooltip) {
                //    createToolTip();
                //}

                updateChart(scope.data);
            };

            //var createToolTip = function () {
            //    lineTip = d3.tip().attr('class', 'd3-tip')
            //        .offset(function () {
            //            return [0, 2]
            //        })
            //        .direction('e')
            //        .style("text-align", "center")
            //        .html(function (d) {
            //            return d[x];
            //        });
            //    d3.select("svg").call(lineTip);
            //};

            var updateChart = function (data) {
                var chart =  d3.select("svg.svg-chart").select("g.chart");
                data = data.map(function (d) {
                    return {
                        unit: d[unit],
                        date: d[date],
                        group: d[group],
                        realDate: formatDate.parse(d[date])
                    };
                });

                data = _.sortBy(data, function (d) { return +d.realDate; });

                //Set up Scales
                var minVal = 0;
                var maxVal = d3.max(data, function (d) { return d.unit; });
                // give 10% buffers on both sides
                maxVal = maxVal + (maxVal * 0.05);
                var yScale = d3.scale.linear()
                    .domain([minVal, maxVal])
                    .range([(h-100), 0]);
                var xScale = d3.time.scale()
                    .range([80, w - 40])
                    .domain(d3.extent(data, function (d) { return d.realDate; }));
                var xAxis = d3.svg.axis()
                    .scale(xScale)
                    .tickFormat(d3.time.format("%m-%y"))
                    .tickPadding(6)
                    .ticks(d3.time.month, 1)
                    .orient("bottom");

                var yAxis = d3.svg.axis()
                    .scale(yScale)
                    .orient("left");

                //chart.append("line")
                //    .attr("x1", xScale(d3.min(data, function(d) { return d.realDate; })))
                //    .attr("x2", xScale(d3.max(data, function(d) { return d.realDate; })))
                //    .attr("y1", yScale(100))
                //    .attr("y2", yScale(100))
                //    .style("stroke", "black")
                //    .style("troke-width", 2)
                //    .style("stroke-dasharray", ("3, 3"));

                var groups = d3.nest()
                    .key(function (d) { return d.group; })
                    .entries(data);

                var line = d3.svg.line()
					.interpolate("linear")
					.x(function (d) { return xScale(d.realDate); })
					.y(function (d) { return yScale(d.unit); });

                 var lines = chart.selectAll("path")
                    .data(groups, function (d) { return d.key; });

                lines.transition()
                    .ease("linear")
                    .duration(1000)
                    .attr("d", function (d) { return line(d.values); });

                lines.enter().append("path")
                    .attr("class", "line")
                    .style("stroke", function (d) { return color(d.key); })
                    .style("fill", "none")
                    .style("stroke-width", 5)
                    .transition()
                    .ease("linear")
                    .duration(1000)
                    .attr("d", function (d) { return line(d.values); });

                //Axes
                d3.select("g.x-axis").call(xAxis);
                d3.select("g.y-axis").call(yAxis);

                lines.exit().remove();

                //Legend
                var legend = chart.selectAll(".legend")
                    .data(_.uniq(_.pluck(data, 'group')))
                  .enter().append("g")
                    .attr("class", "legend")
                    .attr("transform", function (d, i) { return "translate(0," + i * 20 + ")"; });

                legend.append("rect")
                    .attr("x", w - 72)
                    .attr("width", 18)
                    .attr("height", 18)
                    .style("fill", color);

                legend.append("text")
                    .attr("x", w - 96)
                    .attr("y", 9)
                    .attr("dy", ".35em")
                    .style("text-anchor", "end")
                    .text(function (d) { return d; });
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
