using Abp.Application.Services;
using AspNetCoreRateLimitAbpDemo.MultiTenancy.Dto;

namespace AspNetCoreRateLimitAbpDemo.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

