using System;
using System.Threading.Tasks;
using GrpcClient.RpcService;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var checkOrderSvc=new CheckOrderSvc();
            var isSame = await checkOrderSvc.IsSameOrder();
            Console.WriteLine("Hello World!");
        }
    }
}
