angular.module("ehrDashboard")
.directive("bubbleChart", function(){
    return {
        restrict : 'E',
        scope: {
            data: '='
        },
        template: '',
        link: function(scope, elem) {
            var drawChart = function(data) {
                var div = elem[0];
                var w = div.offsetWidth;
                var h = div.offsetHeight;
                var svg = d3.select(div).append("svg").attr("height",h).attr("width",w);
                var graph = svg.append("g").attr("transform","translate(0,20)");
                //line chart
                var xScaleTime = d3.time.scale().range([60,w-50]).domain(d3.extent(data, function(d) { return parseDate(d.date); }));
                var xAxis = d3.svg.axis()
                    .scale(xScaleTime)
                    .ticks(d3.time.month, 3)
                    .tickFormat(d3.time.format("%b"))
                    .orient("bottom");

                var yScaleTime = d3.scale.linear().range([h-50,0])
                    .domain([d3.min(types, function(d) { return d3.min(d.values, function(x) { return x.value; }); }),
                        d3.max(types, function(d) { return d3.max(d.values, function(x) { return x.value; }); })]);
                var yAxis = d3.svg.axis()
                    .scale(yScaleTime)
                    .orient("left");

                var line = d3.svg.line()
                    .interpolate("basis")
                    .x(function(d) { return xScaleTime(parseDate(d.date)); })
                    .y(function(d) { return yScaleTime(d.value); });

                graph.append("g")
                    .attr("class","x-axis")
                    .attr("transform","translate(0,"+(h-60)+")")
                    .call(xAxis);

                svg.append("g")
                    .attr("transform","translate(60,0)")
                    .attr("class", "x-axis")
                    .call(yAxis)
                    .append("text")
                    .attr("transform", "rotate(-90)")
                    .attr("y", 6)
                    .attr("dy", ".71em")
                    .style("text-anchor", "end")
                    .text("USD $");

                var type = svg.selectAll("g.multi")
                    .data(types)
                    .enter().append("g")
                    .attr("class","multi");

                type.append("path")
                    .attr("class", "line")
                    .attr("d", function(d) { return line(d.values); })
                    .style("stroke", function(d) { return color(d.name); })
                    .attr("fill","none");

                type.append("text")
                    .datum(function(d) { return {name: d.name, value: d.values[d.values.length - 1]}; })
                    .attr("transform", function(d) { return "translate(" + (xScaleTime(parseDate(d.value.date))-50) + "," + (yScaleTime(d.value.value)-10) + ")"; })
                    .attr("x", 3)
                    .attr("dy", ".35em")
                    .style("font-size",11)
                    .text(function(d) { return d.name === 'totalMoney' ? "Money Recovered" : "Money Outstanding"; });
            };

            scope.$watch('data', function() {
                if (scope.data){
                    drawChart(scope.data);
                }
            }, true);
        }
    };
});
