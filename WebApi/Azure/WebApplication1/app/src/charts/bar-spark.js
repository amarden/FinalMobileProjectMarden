angular.module("ehrDashboard")
.directive("barSpark", function(){
    return {
        restrict : 'E',
        scope: {
            data: '=',
            prop: '=',
            group: '='
        },
        template: '',
        link: function(scope, elem) {
            var drawChart = function(data,prop,group) {
                var color = d3.scale.category10();
                var div = elem[0];
                var w = div.offsetWidth;
                var h = Math.max(div.offsetHeight, 280);
                var xScale = d3.scale.linear().domain([0, Math.ceil(d3.max(data, function(d) { return d[prop]; }))]).range([0,w/1.6]);
                var yScale = d3.scale.ordinal().domain(data.map(function(d) { return d[group]; })).rangeRoundBands([10,h-50], 0.2);
                var svg = d3.select(div).append("svg").attr("height",h).attr("width",w);
                var graph = svg.append("g").attr("transform","translate(0,20)");
                var g = graph.selectAll("g").data(data)
                    .enter().append("g")
                    .attr("transform", function(d) { return "translate(30,"+yScale(d[group])+")"; });

                var xAxisNum = d3.svg.axis()
                    .scale(xScale)
                    .orient("bottom");

                svg.append("g")
                    .attr("class","x axis")
                    .attr("transform","translate(30," +(h - 25)+")")
                    .call(xAxisNum);

                g.append("rect")
                    .attr("class","spark")
                    .attr("width", function(d) { return xScale(d[prop])})
                    .attr("height", yScale.rangeBand())
                    .style("fill", "steelblue")
                    .style("stroke", "#386890")
                    .style("stroke-width",2);

                g.append("text")
                    .attr("dy", yScale.rangeBand()/2)
                    .attr("dx", 4)
                    .style("fill","white")
                    .style("font-size",12)
                    .style("text-anchor","start")
                    .text(function(d) { return d[group]; });

                g.append("text")
                    .attr("dy", yScale.rangeBand()/2)
                    .style("font-size",15)
                    .style("fill","white")
                    .attr("dx", function(d) { return (xScale(d[prop])-5); })
                    .style("text-anchor","end")
                    .text(function(d) { return d[prop]+"%"; });

                //line chart
                var parseDate = d3.time.format("%m/%d/%Y").parse;
                var xScaleTime = d3.time.scale().range([w/1.5,w-100]).domain(d3.extent(data[0].time, function(d) { return parseDate(d.quarter);}));
                var xAxis = d3.svg.axis()
                    .scale(xScaleTime)
                    .ticks(d3.time.month, 3)
                    .tickFormat(d3.time.format("%b"))
                    .orient("bottom");

                var yTimes = {};

                var line = d3.svg.line()
                    .interpolate("linear")
                    .x(function(d) { return xScaleTime(parseDate(d.quarter)); })
                    .y(function(d) { return yTimes[d.type](d[prop]); });

                g.append("path")
                    .attr("fill","none")
                    .attr("stroke","steelblue")
                    .attr("stroke-width",2)
                    .datum(function(d) {
                        if (!yTimes[d.type]) {
                            yTimes[d.type] = d3.scale.linear()
                                .range([yScale.rangeBand(),10])
                                .domain([-10, 10]);
                                //.domain([(d3.min(d.time, function(d) { return d[prop];})-1), (d3.max(d.time, function(d) { return d[prop];})+1)]);
                        }
                        return d.time;
                    })
                    .attr("d",line);

                g.append("line")
                    .style("stroke-dasharray", ("3", "3"))
                    .style("stroke", "black")
                    .attr("x1", xScaleTime.range()[0])
                    .attr("x2", xScaleTime.range()[1])
                    .attr("y1", function(d) { return yTimes[d.type](0)})
                    .attr("y2", function(d) { return yTimes[d.type](0)});

                g.append("text")
                    .attr("transform", function(d) {
                        return "translate(" + (xScaleTime.range()[1] + 16) + "," + (yTimes[d.type](0) + 5) + ")";
                    })
                    .attr("text-anchor", "middle")
                    .style("font-size", "13px")
                    .text("0%");

                g.selectAll("circle")
                    .data(function(d) { return d.time; })
                .enter().append("circle")
                    .attr("cx", function(d) {
                        return xScaleTime(parseDate(d.quarter));
                    })
                    .attr("cy", function(d) { return yTimes[d.type](d[prop]); })
                    .attr("r", "3px")
                    .attr("fill", "#386890")
                    .attr("stroke", "none");

                svg.append("g")
                    .attr("class","x axis")
                    .attr("transform","translate(30," +(h - 25)+")")
                    .call(xAxis);

                svg.append("text")
                    .attr("y", 15)
                    .attr("x", 30+(w/1.6)/2)
                    .style("text-anchor","middle")
                    .text("Prevalence of Metric - Current Quarter");

                svg.append("text")
                    .attr("y", 15)
                    .style("text-anchor","middle")
                    .attr("x",5+(w/1.6)+((w-(w/1.6))/2))
                    .text("Percent Change From Baseline");
            };

            scope.$watch('data', function() {
                if (scope.data){
                    drawChart(scope.data, scope.prop, scope.group);
                }
            }), true;
        }
    };
});
