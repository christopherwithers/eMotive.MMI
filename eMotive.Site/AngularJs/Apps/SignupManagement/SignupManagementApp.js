var signupManagementApp = angular.module('signupManagementApp', ['signupServices', 'mgcrea.ngStrap']);

signupManagementApp.config(function ($datepickerProvider) {
    angular.extend($datepickerProvider.defaults, {
        dateFormat: 'dd/MM/yyyy',
        startWeek: 1
    });
});

signupManagementApp.controller("myApp", function ($scope, $signupServices, $location) {
    var url = $location.absUrl();

   var id = url.substring(url.lastIndexOf("/") + 1);

   // $scope.groups = $signupServices.getAllGroups();


    $signupServices.getSignup(url.substring(url.lastIndexOf("/") + 1)).then(function(data) {

        if (data.Success) {
            $scope.signup = data.Result[0];
           // alert($scope.signup.AcademicYear);
        } else {
            alert("error!");

        }
    });

 //   alert($scope.signup.AcademicYear);

});