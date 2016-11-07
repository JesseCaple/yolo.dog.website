// WebSocket client-side implementation of JsonClient

if ("WebSocket" in window) {

    var JsonClient = {};
    JsonClient.onOpen = function () { };
    JsonClient.onClose = function () { };
    JsonClient.onMessage = function () { };
    JsonClient.WebSocket = new WebSocket("wss://localhost:44366/YoloWebSocket/");

    JsonClient.Disconnect = function ()
    {
        JsonClient.WebSocket.close(0, "Disconnected");
    };

    JsonClient.WebSocket.onopen = function (e)
    {
        console.log("JsonClient connection opened");
        JsonClient.onOpen(e);
    };

    JsonClient.WebSocket.onmessage = function (event)
    {
        JsonClient.onMessage(JSON.parse(event.data));
    };

    JsonClient.WebSocket.onclose = function (e)
    {
        console.log("JsonClient connection closed: " + e.reason);
        JsonClient.onClose(e);
    };

}
else {

    window.location.replace("Error");

}


