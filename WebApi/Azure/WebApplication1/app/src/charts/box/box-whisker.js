angular.module("ehrDashboard")
.directive("boxWhisker", function(){
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
                var div = elem[0];
                var width = div.offsetWidth / 3 - 5;
                var height = 300;

                var min = -10,
                    max = 110;

                var chart = d3.box()
                    .whiskers(iqr(1.5))
                    .width(width / 3 - 25)
                    .height(height - 15);

                chart.domain([min, max]);

                var svg = d3.select(div).selectAll('svg')
                  .data(data)
                .enter().append("svg")
                  .attr("class", "box")
                  .attr("width", width)
                  .attr("height", height)
                .append("g")
                    .attr("transform", "translate(100,10)")
                  .call(chart);

                svg.append("text")
                    .attr("transform", "translate(" + (width / 3 - 25) / 2 + "," + (height - 15) + ")")
                    .attr("text-anchor", "middle")
                    .style("font-size", "16px")
                    .text(function(d) {
                        return d.group;
                    });
            };
            var iqr = function(k) {
              return function(d, i) {
                var q1 = d.quartiles[0],
                    q3 = d.quartiles[2],
                    iqr = (q3 - q1) * k,
                    i = -1,
                    j = d.length;
                while (d[++i] < q1 - iqr);
                while (d[--j] > q3 + iqr);
                return [i, j];
              };
            }
            scope.$watch('data', function() {
                if (scope.data){
                    drawChart(scope.data, scope.prop, scope.group);
                }
            }), true;
        }
    };
});
