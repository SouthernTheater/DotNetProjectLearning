using System;
using System.Threading.Tasks;
using ConsoleApp1.Notification;
using ConsoleApp1.RequestResponse;
using MediatR;

namespace ConsoleApp1.DoService
{
    public class DoTestService : IDoBaseService
    {
        private readonly IMediator _mediator;
        public DoTestService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task OnStart()
        {
            var ping =new Ping();
            ping.Name = "Tom";
            var response = await _mediator.Send(ping);
            //Console.WriteLine(response); // "Pong"
            var msg=new TestMsg();
            msg.Name = "Jack";
            await _mediator.Publish(msg);
        }

        public async Task OnStop()
        {

        }
    }
}