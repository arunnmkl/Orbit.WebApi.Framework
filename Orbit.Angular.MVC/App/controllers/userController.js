'use strict';
app.controller('userController', ['$scope', 'userService', function ($scope, userService) {
    $scope.name = ' ';
    $scope.username = ' ';
    $scope.securityId = ' ';
    $scope.roles = ' ';
    $scope.userAuthToken = ' ';
    $scope.authenticationType = ' ';
    $scope.timeInSeconds = ' ';
    $scope.userAuthClient = ' ';

    var getTimeFactor = function (timeInSeconds) {
        var minutes = Math.floor((timeInSeconds % 3600) / 60), seconds = timeInSeconds % 60, hours = Math.floor(timeInSeconds / 3600), ret = "";

        if (hours > 0) {
            ret += hours + " hr : " + (minutes < 10 ? "0" : "");
        }

        if (minutes > 0) {
            ret += minutes + " min : " + (seconds < 10 ? "0" : "");
        }

        ret += seconds + " sec";
        return ret;
    };

    userService.getUserDetails().then(function (response) {

        var data = response.data.Value;
        $scope.name = data.Name || $scope.name;
        $scope.username = data.Username || $scope.username;
        $scope.securityId = data.SecurityId || $scope.securityId;
        $scope.roles = data.Roles && data.Roles.length ? data.Roles.join(', ') : 'Not assigned any';
        $scope.userAuthToken = data.UserAuthToken || $scope.userAuthToken;
        $scope.authenticationType = data.AuthenticationType || $scope.authenticationType;
        $scope.timeInSeconds = getTimeFactor(data.TimeInSeconds || $scope.timeInSeconds);
        $scope.userAuthClient = (data.AuthClient || $scope.userAuthClient);

    }, function (error) {
        //alert(error.data.message);
    });
}]);