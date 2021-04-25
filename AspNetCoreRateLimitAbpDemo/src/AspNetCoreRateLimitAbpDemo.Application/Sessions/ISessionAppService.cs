using System.Threading.Tasks;
using Abp.Application.Services;
using AspNetCoreRateLimitAbpDemo.Sessions.Dto;

namespace AspNetCoreRateLimitAbpDemo.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
