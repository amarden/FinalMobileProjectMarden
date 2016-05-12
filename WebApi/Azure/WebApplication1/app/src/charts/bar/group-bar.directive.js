angular.module('ehrDashboard')
    .directive("groupBar", function ($location) {
    return {
        restrict: 'E',
        templateNameSpace: 'svg',
        link: function (scope, elem) {
            var createChart = function (data) {
                var margin = { top: 10, right: 20, bottom: 80, left: 40 };
                var width = elem[0].offsetWidth - margin.right - margin.left;
                var height = elem[0].offsetHeight - margin.top - margin.bottom;

                console.log(width, height);
                var x0 = d3.scale.ordinal()
                    .rangeRoundBands([0, width], 0.1);

                var x1 = d3.scale.ordinal();

                var y = d3.scale.linear()
                    .range([height, 0]);

                var color = d3.scale.ordinal()
                    .range(["red", "blue", "purple"])
                    .domain(["Overhead", "Billable", "Proposal"]);

                var xAxis = d3.svg.axis()
                    .scale(x0)
                    .orient("bottom");

                var yAxis = d3.svg.axis()
                    .scale(y)
                    .orient("left")
                    .tickFormat(d3.format(".2s"));

                var svg = d3.select(elem[0]).append("svg")
                    .attr("width", width + margin.left + margin.right)
                    .attr("height", height + margin.top + margin.bottom)
                  .append("g")
                    .attr("transform", "translate(" + margin.left + "," + margin.top + ")");
                var options = _.uniq(_.pluck(data, 'chargeType')).sort();
                var groupByPerson = _.groupBy(data, 'employee');
                var groupedData = _.map(groupByPerson, function (d, key) {
                    return {
                        name: key,
                        empId: d[0].empId,
                        data: d
                    };
                });

                x0.domain(groupedData.map(function (d) { return d.name; }));
                x1.domain(options).rangeRoundBands([0, x0.rangeBand()]);
                y.domain([0, d3.max(groupedData, function (d) { return d3.max(d.data, function (d) { return d.hours; }); })]);

                svg.append("g")
                  .attr("class", "x axis")
                  .attr("transform", "translate(0," + height + ")")
                  .call(xAxis);

                svg.append("g")
                    .attr("class", "y axis")
                    .call(yAxis)
                  .append("text")
                    .attr("transform", "rotate(-90)")
                    .attr("y", 6)
                    .attr("dy", ".71em")
                    .style("text-anchor", "end")
                    .text("Hours");

                var state = svg.selectAll(".state")
                    .data(groupedData)
                  .enter().append("g")
                    .attr("class", "g")
                    .attr("transform", function (d) { return "translate(" + x0(d.name) + ",0)"; })
                    .on("click", function (d) {
                        console.log(d.empId, d);
                        scope.$apply(function () {
                            $location.path("timesheet/" + d.empId);
                        });
                    });

                state.selectAll("rect")
                    .data(function (d) { return d.data; })
                  .enter().append("rect")
                    .attr("width", x1.rangeBand())
                    .attr("x", function (d) { return x1(d.chargeType); })
                    .attr("y", function (d) { return y(d.hours); })
                    .attr("height", function (d) { return height - y(d.hours); })
                    .style("fill", function (d) { return color(d.chargeType); });

                var legend = svg.selectAll(".legend")
                    .data(options.slice().reverse())
                  .enter().append("g")
                    .attr("class", "legend")
                    .attr("transform", function (d, i) { return "translate(0," + i * 20 + ")"; });

                legend.append("rect")
                    .attr("x", width - 18)
                    .attr("width", 18)
                    .attr("height", 18)
                    .style("fill", color);

                legend.append("text")
                    .attr("x", width - 24)
                    .attr("y", 9)
                    .attr("dy", ".35em")
                    .style("text-anchor", "end")
                    .text(function (d) { return d; });
            };

            scope.$watch('billingData', function () {
                if (scope.billingData) {
                    setTimeout(function () { createChart(scope.billingData); }, 100);
                }
            }, true);
        }
    };
});
