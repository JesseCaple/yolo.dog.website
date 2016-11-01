using TubesWebsite.Controllers;
using mRPC;
using System.Threading.Tasks;

namespace TubesWebsite
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
