using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ConsoleApp1.Notification
{
    public class App1TestMsgHandler: INotificationHandler<TestMsg>
    {
        public Task Handle(TestMsg notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"App1TestMsgHandler get notification,the name is : {notification.Name}");
            return Task.CompletedTask;
        }
    }
}