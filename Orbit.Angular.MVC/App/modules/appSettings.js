var base = '';
(function () {

    'use strict'

    app.constant("AppSettings",
    {
        chatBaseUri: document.getElementById("CHATBASEURI").value,
        apiServiceBaseUri: document.getElementById("APISERVICEBASEURI").value,
        chatBasePath: document.getElementById("CHATBASEPATH").value,
        version: document.getElementById("VERSION").value,
        clientId: 'Default'//'ngAuthApp'
    });

    base = document.getElementById("BASEURI").value

}());