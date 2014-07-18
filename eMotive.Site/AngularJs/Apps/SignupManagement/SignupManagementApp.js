var signupManagementApp = angular.module('signupManagementApp', ['signupServices']);

signupManagementApp.controller("myApp", function ($scope, $signupServices, $location) {
    var url = $location.absUrl();

   var id = url.substring(url.lastIndexOf("/") + 1);

    $scope.groups = $signupServices.getAllGroups();


    $scope.signup = $signupServices.getSignup(id);

    alert($scope.signup.academicyear);
});