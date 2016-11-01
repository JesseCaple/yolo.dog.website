// WebSocket client-side implementation of mRPC

if ("WebSocket" in window)
{
    var mRPC = {};
    var Client = {};
    var Server = {};

    mRPC.Pending = {};
    mRPC.IDCounter = 0;
    mRPC.GenerateID = function ()
    {
        return "i" + mRPC.IDCounter++;
    };

    mRPC.AddRoute = function (controller, action)
    {
        Server[controller][action] = function ()
        {
            var id = mRPC.GenerateID();
            var message = JSON.stringify(
            {
                ID: id,
                Controller: controller,
                Action: action,
                Parameters: [].slice.call(arguments)
            });
            mRPC.Socket.send(message);
            mRPC.Pending[id] = {};
            return mRPC.Pending[id];
        }
    };

    mRPC.Socket = new WebSocket("wss://localhost:44366/mRPC/");

    // socket received a message
    mRPC.Socket.onmessage = function (event)
    {
        var message = JSON.parse(event.data);
        if (message.Intent === "Call")
        {
            // call a client function
            var method = mRPC[message.Controller][message.Action];
            if (typeof method === "function")
            {
                method.apply(this, message.Parameters);
            }
            else
            {
                console.error("missing client RPC method: " +
                    message.Controller + "." + message.Action);
            }
        }
        else if (message.Intent === "Result")
        {
            // message routing finished and is returning a result
            var value = null;
            if (message.Value !== 'undefined' && message.Value !== null)
            {
                value = message.Value;
            }
            var pending = mRPC.Pending[message.ID];
            delete mRPC.Pending[message.ID];
            if (typeof pending.then === "function")
            {
                if (message.hasOwnProperty("Error"))
                {
                    pending.then(null);
                }
                else
                {
                    pending.then(value);
                }
            }
            if (message.hasOwnProperty("Error"))
            {
                console.error(message.Error);
            }
        }
        else if (message.Intent === "Hello")
        {
            // connection established, opening message
            for (var i = 0; i < message.Controllers.length; i++)
            {
                var controller = message.Controllers[i];
                Server[controller] = {};
            }
            for (var i = 0; i < message.Routes.length; i++)
            {
                var route = message.Routes[i];
                mRPC.AddRoute(route.Controller, route.Action);
            }
            if (typeof mRPCReady === "function")
            {
                mRPCReady();
            }
        }
        else if (message.Intent === "undefined" || message.Intent === null)
        {
            console.error("missing message intent");
        }
        else
        {
            console.error("unknown message intent: " + message.Intent);
        }
    };

    // socket closed
    mRPC.Socket.onclose = function (e)
    {
        console.log("mRPC connection closed: " + e.reason);
        if (typeof mRPCClosed === "function")
        {
            mRPCClosed();
        }
    };
}
else
{
    window.location.replace("Error");
}



