using System.Threading.Tasks;
using Abp.Application.Services;
using AspNetCoreRateLimitAbpDemo.Authorization.Accounts.Dto;

namespace AspNetCoreRateLimitAbpDemo.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
