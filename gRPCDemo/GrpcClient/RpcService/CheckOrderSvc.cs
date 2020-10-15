using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcServer;

namespace GrpcClient.RpcService
{
    public class CheckOrderSvc
    {

        private readonly string _grpcUrl = "https://localhost:5001";

        public async Task<bool> IsSameOrder()
        {
            var grpcOption = new GrpcChannelOptions();
            grpcOption.MaxReceiveMessageSize = 100 * 1024 * 1024;
            grpcOption.MaxSendMessageSize = 100 * 1024 * 1024;

            using var channel = GrpcChannel.ForAddress(_grpcUrl, grpcOption);
            var client = new checkOrder.checkOrderClient(channel);
            var request = new OrderRequest();
            request.Name = "李白";
            request.City = "北京";
            request.OrderNumber = "AAAAAAA";
            request.CreationTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow);
            request.PackageList.Add(new PackageRequest
            {
                Weight = 1f,
                OrderNumber = "AAAAAA"
            });

            var reply = await client.IsSameOrderAsync(request);
            return reply.IsSame;
        }
    }
}