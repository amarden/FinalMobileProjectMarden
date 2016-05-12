//Angular controller that governs the business logic on our home page
angular.module("ehrDashboard")
    .controller("MainCtrl", function(Data, Calculate) {
        var vm = this;
        vm.data = {};
        vm.groupDiag = "group";
        vm.unit = "number";
        //Call data service and then process data and assign it to view model
        Data.getData().then(function (data) {
            console.log(data.data);
            vm.processed = Calculate.getMetrics(data.data.patients, data.data.providers);
            console.log(vm.processed);
        });
    });  