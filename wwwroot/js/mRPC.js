// WebSocket client-side implementation of mRPC

if ("WebSocket" in window)
{
    var mRPC = {};
    var Client = {};
    var Server = {};

    mRPC.AddRoute = function (controller, action)
    {
        Server[controller][action] = function ()
        {
            var message = JSON.stringify(
            {
                Controller: controller,
                Action: action,
                Parameters: [].slice.call(arguments)
            });
            mRPC.Socket.send(message);
        };
    };

    mRPC.Ready = false;

    mRPC.Connect = function ()
    {
        mRPC.Socket = new WebSocket("wss://localhost:44366/mRPC/");
    };

    mRPC.Disconnect = function ()
    {
        mRPC.Socket.close(0, "Disconnected");
    };

    mRPC.Connect();

    // socket received a message
    mRPC.Socket.onmessage = function (event)
    {
        var message = JSON.parse(event.data);
        if (mRPC.Ready)
        {
            // call a client function
            var namespace = Client[message.Controller];
            if (typeof namespace !== "undefined")
            {
                var method = Client[message.Controller][message.Action];
                if (typeof method === "function")
                {
                    method.apply(this, message.Parameters);
                    return;
                }
            }
            console.error("missing client function: " + message.Controller + "." + message.Action);
        }
        else
        {
            // add all server RPC-able functions
            for (var i1 = 0; i1 < message.Controllers.length; i1++)
            {
                var controller = message.Controllers[i1];
                Server[controller] = {};
            }
            for (var i2 = 0; i2 < message.Routes.length; i2++)
            {
                var route = message.Routes[i2];
                mRPC.AddRoute(route.Controller, route.Action);
            }
            mRPC.Ready = true;
            if (typeof mRPCReady === "function")
            {
                mRPCReady();
            }
        }
    };

    // socket closed
    mRPC.Socket.onclose = function (e)
    {
        mRPC.Ready = false;
        console.log("mRPC connection closed: " + e.reason);
        if (typeof mRPCClosed === "function")
        {
            mRPCClosed();
        }
        else
        {
            setTimeout(mRPC.Connect, 3000);
        }
    };
}
else
{
    window.location.replace("Error");
}



