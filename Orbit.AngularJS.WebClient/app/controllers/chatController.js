'use strict';
app.controller('chatController', ['$scope', 'chatService', function ($scope, chatService) {

    var chat = chatService.init();

    $scope.data = {
        model: null,
        change: function (toUser) {

            chatService.getChatHistory(toUser).then(function (results) {
                $scope.historyList = results.data;
            }, function (error) {
                alert(error.data.message);
            });

        },
        availableOptions: []
    };
    $scope.historyList = [];

    chatService.getChatUsers().then(function (results) {
        $scope.data.availableOptions = results.data;
    }, function (error) {
        //alert(error.data.message);
    });

}]);