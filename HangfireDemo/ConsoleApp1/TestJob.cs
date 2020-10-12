using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class TestJob
    {

        [DisplayName("任务{1}")]
        public async Task CallUrl(string url, string jobName)
        {
            HttpClient client = new HttpClient();
            var reps = await client.GetAsync(url);
        }
    }
}