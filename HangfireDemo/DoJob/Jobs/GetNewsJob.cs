using Hangfire;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;

namespace DoJob.Jobs
{
    public class GetNewsJob
    {
        private readonly ILogger<GetNewsJob> _logger;
        //private readonly PerformContext _contextj;

        public GetNewsJob(
            ILogger<GetNewsJob> logger
            //PerformContext contextj
            )
        {
            _logger = logger;
            //_contextj = contextj;
        }

        [Queue("news")]
        [DisplayName("后台任务:{1}")]
        public async Task GetNewsByUrl(string url, string jobName)
        {

            HttpClient client = new HttpClient();
            var reps = await client.GetAsync(url);
            if (reps.IsSuccessStatusCode)
            {
                var respStr = await reps.Content.ReadAsStringAsync();
                _logger.LogInformation(respStr);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="_contextj">执行的时候填null,Hangfire.Console会自动注入该参数</param>
        /// <returns></returns>
        [Queue("news")]
        [DisplayName("后台任务:{0}")]
        public async Task GetBaiduNews(string jobName, PerformContext _contextj)
        {
            string url = "https://www.baidu.com";
            HttpClient client = new HttpClient();
            var reps = await client.GetAsync(url);
            if (reps.IsSuccessStatusCode)
            {
                var respStr = await reps.Content.ReadAsStringAsync();
                //_logger.LogInformation(respStr);
                //_contextj.SetTextColor(ConsoleTextColor.Red);
                _contextj.WriteLine(respStr);
                //_contextj.ResetTextColor();
                //_contextj.WriteLine(ConsoleTextColor.Blue,respStr);
            }
        }

    }
}