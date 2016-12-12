'use strict';
app.controller('indexController', ['$scope', '$location', 'authService', 'AclService', function ($scope, $location, authService, AclService) {

    $scope.logOut = function () {
        authService.signOut();
        AclService.flushRoles();
        AclService.setAbilities({});
        $location.path('/home');
    }

    $scope.authentication = authService.authentication;
    $scope.canToken = AclService.can;

}]);