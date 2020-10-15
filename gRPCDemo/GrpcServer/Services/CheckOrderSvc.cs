using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcServer
{
    public class CheckOrderSvc : checkOrder.checkOrderBase
    {
        private readonly ILogger<CheckOrderSvc> _logger;
        public CheckOrderSvc(ILogger<CheckOrderSvc> logger)
        {
            _logger = logger;
        }

        public override Task<OrderReply> IsSameOrder(OrderRequest request, ServerCallContext context)
        {
            #region 业务逻辑
            //创建时间转换
            DateTime CreationTime = request.CreationTime.ToDateTime();
            var packageList= request.PackageList.ToList();
            foreach (var packageRequest in packageList)
            {
                var weight = packageRequest.Weight;
            }
            #endregion
            var reply=new OrderReply();
            reply.IsSame = false;
            reply.Message = "不是同一订单";
            return Task.FromResult(reply);
        }
    }
}
