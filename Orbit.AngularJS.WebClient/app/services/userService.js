'use strict';
app.factory('userService', ['$http', 'ngAuthSettings', function ($http, ngAuthSettings) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;

    var userServiceFactory = {};

    var _getUserDetails = function () {

        return $http.get(serviceBase + 'api/User/details').then(function (results) {
            return results;
        });
    };

    userServiceFactory.getUserDetails = _getUserDetails;

    return userServiceFactory;

}]);