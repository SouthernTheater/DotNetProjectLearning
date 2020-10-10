using System;
using System.Net.Http;
using System.Threading.Tasks;
using Common;
using DotNetCore.CAP;

namespace ConsoleApp1.IntegrationEvent
{

    public class EventSubscriberService : IEventSubscriberService, ICapSubscribe
    {

        private readonly IHttpClientFactory _clientFactory;
        public EventSubscriberService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [CapSubscribe("xxx.services.show.time")]
        public void CheckReceivedMessage(DateTime datetime)
        {
            Console.WriteLine("事件收到消息：" + datetime);
        }

        [CapSubscribe(GlobalConsts.AddUserActionLogEvent)]
        public async Task AddUserActionLogEventHandler(string actionLog)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.qq.com");
            //request.Headers.Add("Accept", "application/vnd.github.v3+json");
            request.Headers.Add("User-Agent", "HttpClientFactory-Sample");
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseStr) && responseStr.Length > 100)
                    Console.WriteLine(responseStr.Substring(0, 100));
            }

            Console.WriteLine("事件收到消息：" + actionLog);
        }
    }
}