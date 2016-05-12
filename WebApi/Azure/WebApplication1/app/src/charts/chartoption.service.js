angular.module('ehrDashboard')
    .factory("ChartOptions", function () {
    return {
        defaults: [
        { 'propName': 'margin', 'defaultValue': { "top": 0, "left": 0, "right": 0, "bottom": 0 } },
        { 'propName': 'colorScale', 'defaultValue': d3.scale.category10() },
        { 'propName': 'drawLegend', 'defaultValue': false },
        { 'propName': 'drawYAxis', 'defaultValue': false },
        { 'propName': 'tooltip', 'defaultValue': false },
        { 'propName': 'dateFormat', 'defaultValue': d3.time.format("%m/%d/%Y") },
        { 'propName': 'tickFormat', 'defaultValue': d3.time.month },
        { 'propName': 'tickSpacing', 'defaultValue': 1 },
        { 'propName': 'dateAxisFormat', 'defaultValue': d3.time.format("%m-%y") },
        { 'propName': 'timeUnit', 'defaultValue': 'month' },
        { 'propName': 'min', 'defaultValue': 'min' },
        { 'propName': 'max', 'defaultValue': 'max' },
        { 'propName': 'yScaleLabel', 'defaultValue': '' },
        { 'propName': 'title', 'defaultValue': '' },
        { 'propName': 'onClick', 'defaultValue': '' },
        ],
        generateOptions: function (options) {
            var controls = options ? options : {};
            this.defaults.forEach(function (d) {
                if (!controls.hasOwnProperty(d.propName)) {
                    controls[d.propName] = d.defaultValue;
                }
            });
            return controls;
        }
    };
});
