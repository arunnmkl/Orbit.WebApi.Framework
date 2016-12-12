var app = angular.module('AngularAuthApp', ['ngRoute', 'LocalStorageModule', 'angular-loading-bar', 'mm.acl', 'ui.bootstrap']);

app.config(function ($routeProvider) {

    $routeProvider.when("/home", {
        controller: "homeController",
        templateUrl: "app/views/home.html"
    });

    $routeProvider.when("/login", {
        controller: "loginController",
        templateUrl: "app/views/login.html"
    });

    $routeProvider.when("/signup", {
        controller: "signupController",
        templateUrl: "app/views/signup.html"
    });

    $routeProvider.when("/orders", {
        controller: "ordersController",
        templateUrl: "app/views/orders.html"
    });

    $routeProvider.when("/refresh", {
        controller: "refreshController",
        templateUrl: "app/views/refresh.html",
        resolve: {
            'acl': ['$q', 'AclService', function ($q, AclService) {
                if (AclService.can('Read_RefreshToken')) {
                    // Has proper permissions
                    return true;
                } else {
                    // Does not have permission
                    return $q.reject('unauthorized');
                }
            }]
        }
    });

    $routeProvider.when("/tokens", {
        controller: "tokensManagerController",
        templateUrl: "app/views/tokens.html"
    });

    $routeProvider.when("/associate", {
        controller: "associateController",
        templateUrl: "app/views/associate.html"
    });

    $routeProvider.when("/welcome", {
        controller: "userController",
        templateUrl: "app/views/user.html"
    });

    $routeProvider.when("/unauthorized", {
        templateUrl: "app/views/unauthorized.html"
    });

    $routeProvider.when("/chat", {
        controller: "chatController",
        templateUrl: "app/views/chat.html"
    });

    $routeProvider.otherwise({ redirectTo: "/home" });

});

app.constant('ngAuthSettings', {
    apiServiceBaseUri: appConfig.serviceBase,
    clientId: 'Default'//'ngAuthApp'
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.run(['$rootScope', 'authService', function ($rootScope, authService) {
    authService.fillAuthData();
    authService.isAuthorized().then(function (response) {
        $rootScope.$emit('userAuthorized', response.data);
    });
}]);

app.config(['AclServiceProvider', function (AclServiceProvider) {
    var myConfig = {
        storage: 'localStorage',
        storageKey: 'AppAcl'
    };
    AclServiceProvider.config(myConfig);
    AclServiceProvider.resume();
}]);

app.run(['$rootScope', 'AclService', 'authService', function ($rootScope, AclService, authService) {
    $rootScope.$on('userAuthorized', function (event, isAuthorized) {
        if (isAuthorized) {
            // Web storage record did not exist, we'll have to build it from scratch
            authService.getUserGroupPermission().then(function (response) {
                for (let permission of response.data) {
                    AclService.addAbility(permission.ResourceName, (permission.PermissionString + '_' + permission.ResourceName));

                // Get the user role, and add it to AclService
                    AclService.attachRole(permission.ResourceName);

                // Get ACL data, and add it to AclService
                //AclService.setAbilities(permission.PermissionString);
                }
            });
        }
        else {
            AclService.flushRoles();
            AclService.setAbilities({});
        }
    });
}]);

app.run(['$rootScope', '$location', function ($rootScope, $location) {
    // If the route change failed due to our "Unauthorized" error, redirect them
    $rootScope.$on('$routeChangeError', function (event, current, previous, rejection) {
        if (rejection === 'unauthorized') {
            $location.path('/unauthorized');
        }
    })
}]);
