Client.Home = {};

Client.Home.Test = function (number, string)
{
    console.log("server called me");
    console.log("number: " + number + "   string:" + string);
};

mRPCReady = function ()
{
    //Server.Home.Test("im calling the server");
};
