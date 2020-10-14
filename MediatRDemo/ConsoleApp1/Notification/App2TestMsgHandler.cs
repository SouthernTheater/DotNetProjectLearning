using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ConsoleApp1.Notification
{
    public class App2TestMsgHandler : INotificationHandler<TestMsg>
    {
        public Task Handle(TestMsg notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"App2TestMsgHandler get notification,the name is : {notification.Name}");
            return Task.CompletedTask;
        }
    }
}