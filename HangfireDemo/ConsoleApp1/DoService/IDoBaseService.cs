using System.Threading.Tasks;

namespace ConsoleApp1.DoService
{
    public interface IDoBaseService
    {
        /// <summary>
        /// 服务启动时执行
        /// </summary>
        Task OnStart();

        /// <summary>
        /// 服务停止时执行
        /// </summary>
        Task OnStop();

    }
}