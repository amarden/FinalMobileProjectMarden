angular.module("ehrDashboard")
    .factory("BarOptions", function () {
        return {
            defaults: [
            { 'propName': 'margin', 'defaultValue': { "top": 50, "left": 100, "right": 150, "bottom": 50 } },
            { 'propName': 'chartTitle', 'defaultValue': ""},
            { 'propName': 'color', 'defaultValue': 'steelblue' },
            { 'propName': 'colorBy', 'defaultValue': 'none' },
            { 'propName': 'tooltip', 'defaultValue': true },
            { 'propName': 'fitToHeight', 'defaultValue': true },
            { 'propName': 'tickNumbers', 'defaultValue': null },
            { 'propName': 'width', 'defaultValue': null },
            { 'propName': 'height', 'defaultValue': null },
            { 'propName': 'maxX', 'defaultValue': null }
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
