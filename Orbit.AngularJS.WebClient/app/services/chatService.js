app.factory('chatService', ['localStorageService', '$http', 'ngAuthSettings', function (localStorageService, $http, ngAuthSettings) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;

    var init = function () {
        var authData = localStorageService.get('authorizationData');

        $.signalR.ajaxDefaults.headers = { Authorization: authData.tokenType + " " + authData.token };

        //Set the hubs URL for the connection
        $.connection.hub.url = appConfig.chatUrl;

        $.connection.hub.qs = { 'BearerToken': authData.token };

        // Declare a proxy to reference the hub.
        var chat = $.connection.chatHub,
            timer = 0;

        chat.connection.stop();

        // Create a function that the hub can call to broadcast messages.
        chat.client.addMessage = function (name, message) {
            // Html encode display name and message.
            var encodedName = $('<div />').text(name).html();
            var encodedMsg = $('<div />').text(message).html();
            // Add the message to the page.
            $('#discussion').append('<li class="ping"><strong>Received addMessage' + encodedName
                + '</strong>&nbsp;&nbsp;' + encodedMsg + '</li>');
        };

        // Create a function that the hub can call to broadcast messages.
        chat.client.tick = function (connectionId, name, message) {
            // Html encode display name and message.
            var encodedConnectionId = $('<div />').text(connectionId).html();
            var encodedName = $('<div />').text(name).html();
            var encodedMsg = $('<div />').text(message).html();
            // Add the message to the page.
            $('#discussion').append("<li><strong>Server tick's:: " + encodedConnectionId + '</strong>&nbsp;&nbsp;' + encodedName + '&nbsp;&nbsp;' + encodedMsg + '</li>');

            //window.scrollTo(0, document.body.scrollHeight);
        };

        chat.client.groupHandShake = function (username) {
            // Html encode display name and message.
            var encodedName = $('<div />').text(username).html();
            // Add the message to the page.
            $('#discussion').append('<li><strong>Handshake from user: ' + encodedName + '</strong></li>');
        };

        chat.client.sendHelloObject = function (hello) {
            // Html encode display name and message.
            var encodedName = $('<div />').text(hello.Molly).html();
            var encodedMsg = $('<div />').text(hello.Age).html();
            // Add the message to the page.
            $('#discussion').append('<li><strong>Received sendHelloObject:' + encodedName + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
        };

        chat.client.heartbeat = function () {
            // Html encode display name and message.
            var encodedName = $('<div />').text("heartbeat").html();

            // Add the message to the page.
            $('#discussion').append('<li><strong>Received ' + encodedName + '</strong></li>');
        };

        chat.client.onlineUserCount = function (userCount) {
            // Html encode display name and message.
            var encodedUserCount = $('<div />').text('Online(' + userCount + ')').html();
            $('.onlineCount').empty().fadeOut('slow', function () {
                $('.onlineCount').append('<strong>' + encodedUserCount + '</strong>');
                $('.onlineCount').fadeIn();
            });
        };

        chat.client.online = function (user) {
            if (user) {
                // Html encode display name and message.
                var encodedName = $('<div />').text(user.UserName).html();

                // Add the message to the page.
                $('#discussion').append('<li class="online"><strong>' + encodedName + '</strong> is now available.</li>');
            }
        };

        chat.client.offline = function (user) {
            if (user) {
                // Html encode display name and message.
                var encodedName = $('<div />').text(user.UserName).html();

                // Add the message to the page.
                $('#discussion').append('<li class="offline">User <strong>' + encodedName + '</strong> has gone offline.</li>');
            }
        };

        $.connection.hub.disconnected(function () {
            deactive();

            // Html encode display name and message.
            var encodedName = $('<div />').text('You are successfully disconnected from the server!!!').html();

            // Add the message to the page.
            $('#discussion').append('<li><strong>Deregistered:: ' + encodedName + '</strong></li>');

            $('#sendmessage').unbind('click');
            $('#heartbeat').unbind('click');
            $('#sendHelloObject').unbind('click');
            $('#groupHandShake').unbind('click');
            $('#disconnect').unbind('click');
            $('#toggleTimer').unbind('click');

            $('#reconnect').click(function (e) {
                e.preventDefault();
                Connect(true);
                $('#discussion').empty();
            });

        });

        function Connect(isReconnecting) {
            active();
            // Start the connection.
            $.connection.hub.start().done(function () {
                tickTimer();
                chat.server.register().done(function (user) {
                    if (user) {
                        var userName = user.UserName,
                            groupName = user.Groups ? user.Groups.join(', ') : '';
                        setUserInfo(userName, groupName);

                        var welcomeText = 'Hi ' + user.UserName + '!!!';
                        if (isReconnecting) {
                            welcomeText = 'Welcome back ' + user.UserName + '!!!';
                        }

                        if (!userName && user.UserName && groupName) {
                            $('#displayname').val(user.UserName);
                            $('#username').html('Welcome <strong>' + user.UserName + '!!!</strong> and you are in <strong>' + groupName + '</strong> group.');
                        }
                        else if (!userName && user.UserName) {
                            $('#displayname').val(user.UserName);
                            $('#username').html('Welcome <strong>' + user.UserName + '!!!</strong>');
                        }

                        // Html encode display name and message.
                        var encodedName = $('<div />').text(welcomeText).html();

                        // Add the message to the page.
                        $('#discussion').append('<li><strong>Registered:: ' + encodedName + '</strong></li>');

                        // Html encode display name and message.
                        encodedName = $('<div />').text('Your Connection Id --> ' + user.ConnectionId).html();

                        // Add the message to the page.
                        $('#discussion').append('<li><strong>:: ' + encodedName + '</strong></li>');

                    }
                });

                $('#sendmessage').click(function () {
                    var sid = $('#chatUser').val()
                    // Call the Send method on the hub.
                    chat.server.addMessage($('#displayname').val(), $('#message').val(), sid ? sid : null);
                    // Clear text box and reset focus for next comment.
                    $('#message').val('').focus();
                });

                $('#heartbeat').click(function () {
                    // Call the Send method on the hub.
                    chat.server.heartbeat();
                    // Clear text box and reset focus for next comment.
                    $('#message').val('').focus();
                });

                $('#sendHelloObject').click(function () {
                    // Call the Send method on the hub.
                    chat.server.sendHelloObject({ Age: 2, Molly: $('#message').val() });
                    // Clear text box and reset focus for next comment.
                    $('#message').val('').focus();
                });

                $('#groupHandShake').click(function () {
                    // Call the Send method on the hub.
                    chat.server.groupHandShake();
                    // Clear text box and reset focus for next comment.
                    $('#message').val('').focus();
                });

                $('#disconnect').click(function (e) {
                    e.preventDefault();
                    var stop = $.connection.chatHub.connection.stop();
                });

                $('#reconnect').unbind('click');

                $('#toggleTimer').click(function () {
                    var toggle = $('#pingTimer').val();
                    if (toggle == 'false') {
                        // Call the Send method on the hub.
                        chat.server.setTimer(5);
                        $('#pingTimer').val('true');
                        $('#toggleTimer').val('Turn Off Timer');
                    } else {
                        chat.server.setTimer(0);
                        $('#pingTimer').val('false');
                        $('#toggleTimer').val('Turn On Timer');
                    }
                });
            });
        }

        function active() {
            $('#heartbeat').attr('disabled', false);
            $('#sendHelloObject').attr('disabled', false);
            $('#groupHandShake').attr('disabled', false);
            $('#message').attr('disabled', false);
            $('#sendmessage').attr('disabled', false);
            $('#disconnect').attr('disabled', false);
            $('#toggleTimer').attr('disabled', false);

            $('#reconnect').attr('disabled', true);
            $('.countdown').fadeIn();
            $('.circleBase').show();
        }

        function deactive() {
            $('#heartbeat').attr('disabled', true);
            $('#sendHelloObject').attr('disabled', true);
            $('#groupHandShake').attr('disabled', true);
            $('#message').attr('disabled', true);
            $('#sendmessage').attr('disabled', true);
            $('#disconnect').attr('disabled', true);
            $('#toggleTimer').attr('disabled', true);

            $('#reconnect').attr('disabled', false);

            clearInterval(timer);
            $('.countdown').fadeOut();
            $('.circleBase').fadeOut();
            $('#timer').empty();
            $('.onlineCount').empty().fadeOut('fast');
        }

        function getTimeFactor(timeInSeconds) {
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

        function setUserInfo(name, groupName) {

            if (name && groupName) {
                $('#displayname').val(name);
                $('#groupname').val(groupName);
                $('#username').html('Welcome <strong>' + name + '!!!</strong> and you are in <strong>' + groupName + '</strong> group.');
            }
            else if (name) {
                $('#displayname').val(name);
                $('#username').html('Welcome <strong>' + name + '!!!</strong>');
                $('#groupHandShake').fadeOut();

            }
            else if (groupName) {
                $('#groupname').val(groupName);
                $('#username').html('You are in <strong>' + groupName + '</strong> group.');
            }
        }

        function tickTimer() {
            var tcount = 600,
                updateTimer = function () {
                    if (tcount % 5 > 0) {
                        $('#timer').fadeOut('fast', function () {
                            $('#disconnectTimer').text(getTimeFactor(tcount));
                            $('#timer').text(tcount % 5);
                            $('#timer').fadeIn();
                            tcount--;
                        });
                    }
                    else if (tcount % 5 === 0) {
                        $('#timer').fadeOut('fast', function () {
                            $('#disconnectTimer').text(getTimeFactor(tcount));

                            if (tcount !== 600) {
                                $('#timer').text("Tick's!!");
                            }
                            else {
                                $('#timer').text("Start!!");
                            }
                            $('#timer').fadeIn();
                            tcount--;
                        });
                    }
                    else {
                        $('#timer').fadeOut().empty();
                        $('#disconnectTimer').empty();
                        clearInterval(timer);
                        $.connection.chatHub.connection.stop();
                    }
                };
            updateTimer();
            $('#disconnectTimer').text(getTimeFactor(tcount));
            timer = setInterval(updateTimer, 1000);
        }

        // Set initial focus to message input box.
        $('#message').focus();

        // connect to the signalR hub
        Connect();

        return chat;
    }
    , logout = function () {
        $.connection.chatHub.connection.stop();
    }
    , getChatUsers = function () {
        return $http.get(serviceBase + 'api/chat/associatedusers').then(function (results) {
            return results;
        });
    }
    , getChatHistory = function (toUser) {
        return $http.get(serviceBase + 'api/chat/history/' + toUser).then(function (results) {
            return results;
        });
    };

    return {
        init: init,
        get chatHub() {
            return $.connection.chatHub;
        },
        logout: logout,
        getChatUsers: getChatUsers,
        getChatHistory: getChatHistory
    };
}]);