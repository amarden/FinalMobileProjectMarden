//Angular Service used to calculate all of the information we show on the dashboard
angular.module("ehrDashboard")
    .service("Calculate", function () {
        this.getMetrics = function (patient, provider) {
            var metric = {};
            metric.ageAverage = patient
                .map(function (d) { return d.Age })
                .reduce(function (a, b) { return a + b; })
                / patient.length;

            metric.chatAverage = patient
                .map(function (d) { return d.chatCount })
                .reduce(function (a, b) { return a + b; })
                / patient.length;

            metric.imageAverage = patient
                .map(function (d) { return d.imageCount })
                .reduce(function (a, b) { return a + b; })
                / patient.length;

            metric.providerAverage = patient
                .map(function (d) { return d.providerCount })
                .reduce(function (a, b) { return a + b; })
                / patient.length;

            var groupDiag = _.groupBy(patient, "diagnosis");
            metric.topDiagnoses = _.map(groupDiag, function (data, key) {
                return {
                    group:  key.length > 32 ? key.substring(0,32)+"..." : key,
                    number: data.length
                };
            });

            var procDiag = _.groupBy(_.flatten(patient.map(function (d) { return d.procedures; })));
            metric.topProcedures = _.map(procDiag, function (data, key) {
                return {
                    group: key,
                    number: data.length
                };
            });

            var roleGroup = _.groupBy(provider.map(function (d) { return d.Role; }));
            metric.roleCount = _.map(roleGroup, function (data, key) {
                return {
                    group: key.length > 32 ? key.substring(0,32)+"..." : key,
                    number: data.length
                };
            });

            var statusGroup = _.groupBy(patient.map(function (d) { return d.MedicalStatus; }));
            metric.statusCount = _.map(statusGroup, function (data, key) {
                return {
                    group: key,
                    number: data.length
                };
            });

            return metric;
        };
    });