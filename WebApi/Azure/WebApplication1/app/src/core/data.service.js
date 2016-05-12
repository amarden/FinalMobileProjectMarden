//Data Service that makes the api call to our server to get dashboard data
angular.module("ehrDashboard")
    .service("Data", function($http){
        this.getData = function () {
            return $http.get("api/Dashboard")
                .then(function (data) {
                    return data;
                });
        };
    });
