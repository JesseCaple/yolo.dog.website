// WebSocket client-side implementation of MVCRPC

if ("WebSocket" in window)
{
    var MVCPRC = {};
    var Client = {};
    var Server = {};

    MVCPRC.Pending = {};
    MVCPRC.IDCounter = 0;
    MVCPRC.GenerateID = function ()
    {
        return "i" + MVCPRC.IDCounter++;
    };

    MVCPRC.Socket = new WebSocket("wss://localhost:44366/rpc/");

    // socket opened
    MVCPRC.Socket.onopen = function ()
    {
        var controllers = [];
        for (namespace in Client)
        {
            controllers.push(namespace);
        }
        var message = JSON.stringify(
        {
            Controllers: controllers
        });
        MVCPRC.Socket.send(message);
    };

    // socket received a message
    MVCPRC.Socket.onmessage = function (event)
    {
        var message = JSON.parse(event.data);
        if (message.Intent === "Call")
        {
            // call a client function
            var method = MVCPRC[message.Controller][message.Action];
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
            var pending = MVCPRC.Pending[message.ID];
            delete MVCPRC.Pending[message.ID];
            if (typeof pending.then === "function")
            {
                if (message.hasOwnProperty("Error"))
                {
                    pending.then(false);
                }
                else
                {
                    pending.then(true, value);
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
            var rpcfunction = function ()
            {
                var id = MVCPRC.GenerateID();
                var message = JSON.stringify(
                {
                    ID: id,
                    Controller: controller,
                    Action: action,
                    Parameters: arguments
                });
                MVCPRC.Socket.send(message);
                MVCPRC.Pending[id] = {};
                return MVCPRC.Pending[id];
            };
            for (controller in message.Controllers)
            {
                Server[controller] = {};
            }
            for (route in message.Routes)
            {
                Server[route.Controller][route.action] = rpcfunction;
            }
            if (typeof RPCReady === "function")
            {
                RPCReady();
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
    MVCPRC.Socket.onclose = function (e)
    {
        console.log("tube closed: " + e.reason);
        if (typeof RPCClosed === "function")
        {
            RPCClosed();
        }
    };
}
else
{
    window.location.replace("Error");
}



