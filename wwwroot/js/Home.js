JsonClient.onOpen = function ()
{

};

JsonClient.onMessage = function (message)
{
    console.log(message.text);
};