angular.module("ehrDashboard")
    .factory("LineOptions", function () {
        return {
            defaults: [
            { 'propName': 'margin', 'defaultValue': { "top": 0, "left": 0, "right": 0, "bottom": 0 } },
            { 'propName': 'color', 'defaultValue': 'steelblue' },
            { 'propName': 'xAxisFreq', 'defaultValue': '' },
            { 'propName': 'yMax', 'defaultValue': '' },
            { 'propName': 'colorBy', 'defaultValue': 'none' },
            { 'propName': 'tooltip', 'defaultValue': true },
            { 'propName': 'fitToHeight', 'defaultValue': true },
            { 'propName': 'radius', 'defaultValue': 3 },
            { 'propName': 'width', 'defaultValue': null },
            { 'propName': 'height', 'defaultValue': null },
            {'propName': 'title', 'defaultValue': ''}
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
