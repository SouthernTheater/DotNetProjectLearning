using Abp.Application.Services.Dto;

namespace AspNetCoreRateLimitAbpDemo.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

