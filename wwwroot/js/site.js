Client.Home = {};

Client.Home.Test = function (number, string)
{
    console.log("number: " + number + ", string: " + string);
};

Client.Home.Test2 = function ()
{
    console.log("test 2 called");
};


mRPCReady = function ()
{
    var obj = { value1: 1, value2: "test?" };
    Server.Home.Test(obj);
};
