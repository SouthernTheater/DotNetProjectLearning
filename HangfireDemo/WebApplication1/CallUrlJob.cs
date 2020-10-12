using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class CallUrlJob
    {
        //[DisplayName("调用Url:{0} ,任务名{1}")]
        [DisplayName("任务{1}")]
        public async Task Call(string url,string jobName)
        {
            HttpClient client = new HttpClient();
            var reps = await client.GetAsync(url);
        }
    }
}