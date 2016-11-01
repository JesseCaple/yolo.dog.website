using yolo.dog.website.Controllers;
using mRPC;
using System.Threading.Tasks;

namespace yolo.dog.website
{
    public static class RPC
    {
        public static async Task<RPCResult>
            Test(this RPCManager<HomeController> manager, string name)
        {
            return await manager.DynamicRPC.Test(name);
        }

        public static async Task<RPCResult>
            Test(this RPCConnection<HomeController> client, string name)
        {
            return await client.DynamicRPC.Test(name);
        }
    }
}
