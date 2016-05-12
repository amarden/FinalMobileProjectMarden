//Bootstraps our angular app
angular.module('ehrDashboard', [
    'ngAnimate',
    'ngAria',
    'ngMaterial',
    //'ui-router'
])
.config(function ($mdThemingProvider) {
    $mdThemingProvider.theme('default')
      .primaryPalette('blue')
      .accentPalette('green');
})
