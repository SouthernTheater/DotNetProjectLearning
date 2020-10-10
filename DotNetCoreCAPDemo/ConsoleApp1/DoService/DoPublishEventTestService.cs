using System;
using System.Threading.Tasks;
using Common;
using DotNetCore.CAP;

namespace ConsoleApp1.DoService
{
    public class DoPublishEventTestService: IBaseService
    {
        private readonly ICapPublisher _capBus;

        public DoPublishEventTestService(
            ICapPublisher capPublisher
        )
        {
            _capBus = capPublisher;
        }

        public async Task OnStart()
        {
            var msg = DateTime.Now;
            //await _capBus.PublishAsync("xxx.services.show.time", msg);
            await _capBus.PublishAsync(GlobalConsts.AddUserActionLogEvent, "用户登录，IP：192.168.1.100");
        }

        public async Task OnStop()
        {

        }
    }
}