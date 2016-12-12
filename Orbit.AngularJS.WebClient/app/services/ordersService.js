'use strict';
app.factory('ordersService', ['$http', 'ngAuthSettings', function ($http, ngAuthSettings) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;

    var ordersServiceFactory = {};

    var _createOrder = function (order) {

        return $http.post(order).then(function (results) {
            return results;
        });
    }

    var _getOrders = function () {

        return $http.get(serviceBase + 'api/orders/all').then(function (results) {
            return results;
        });
    };

    var _updateOrder = function (order) {

        return $http.put(order).then(function (results) {
            return results;
        });
    }

    var _deleteOrder = function (orderId) {

        return $http.delete(serviceBase + 'api/orders/' + orderId).then(function (results) {
            return results;
        });
    }

    ordersServiceFactory.createOrder = _createOrder;
    ordersServiceFactory.getOrders = _getOrders;
    ordersServiceFactory.updateOrder = _updateOrder;
    ordersServiceFactory.deleteOrder = _deleteOrder;

    return ordersServiceFactory;

}]);