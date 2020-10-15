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
            #region ҵ���߼�
            //����ʱ��ת��
            DateTime CreationTime = request.CreationTime.ToDateTime();
            var packageList= request.PackageList.ToList();
            foreach (var packageRequest in packageList)
            {
                var weight = packageRequest.Weight;
            }
            #endregion
            var reply=new OrderReply();
            reply.IsSame = false;
            reply.Message = "����ͬһ����";
            return Task.FromResult(reply);
        }
    }
}
