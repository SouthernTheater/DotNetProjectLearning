using System;
using System.Threading.Tasks;

namespace ConsoleApp1.IntegrationEvent
{
    public interface IEventSubscriberService
    {
        void CheckReceivedMessage(DateTime datetime);

        Task AddUserActionLogEventHandler(string actionLog);
    }
}