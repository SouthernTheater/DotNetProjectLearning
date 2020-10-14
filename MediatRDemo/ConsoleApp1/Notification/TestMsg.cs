using MediatR;

namespace ConsoleApp1.Notification
{
    public class TestMsg : INotification
    {
        public string Name { get; set; }
    }
}