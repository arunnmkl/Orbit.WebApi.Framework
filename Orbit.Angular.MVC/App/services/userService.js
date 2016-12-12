'use strict';
app.factory('userService', ['$http', 'AppSettings', function ($http, AppSettings) {

    var serviceBase = AppSettings.apiServiceBaseUri;

    var userServiceFactory = {};

    var _getUserDetails = function () {

        return $http.get(serviceBase + 'api/User/details').then(function (results) {
            return results;
        });
    };

    userServiceFactory.getUserDetails = _getUserDetails;

    return userServiceFactory;

}]);