'use strict';
app.controller('ordersController', ['$scope', '$uibModal', 'ordersService', function ($scope, $modal, ordersService) {

    $scope.orders = [];
    $scope.removeRow = function deleteModal(order) {
        $modal.open({
            templateUrl: 'app/views/deletemodal.html',
            controller: ['$uibModalInstance', 'ordersService', 'orders', 'order', DeleteModalCtrl],
            controllerAs: 'vm',
            resolve: {
                orders: function () { return $scope.orders },
                order: function () { return order; }
            }
        });
    }

    function DeleteModalCtrl($modalInstance, ordersService, orders, order) {
        var vm = this;

        vm.order = order;
        vm.deleteOrder = deleteOrder;

        function deleteOrder() { 
            ordersService.deleteOrder(order.OrderID).then(function (results) {

                ordersService.getOrders().then(function (results) {

                    $scope.orders = results.data;

                }, function (error) {
                    alert(error.data);
                });

            }, function (error) {
                $modal.open({
                    templateUrl: 'app/views/errormodal.html',
                    controller: ['$uibModalInstance', 'error', function ($uibModalInstance, error) { this.Error = error; }],
                    controllerAs: 'vm',
                    resolve: {
                        error: function () { return error.data; }
                    }
                });
            });
            $modalInstance.close();
        }
    }

    ordersService.getOrders().then(function (results) {

        $scope.orders = results.data;

    }, function (error) {
        //alert(error.data.message);
    });

}]);