using Abp.AutoMapper;
using AspNetCoreRateLimitAbpDemo.Authentication.External;

namespace AspNetCoreRateLimitAbpDemo.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public string Name { get; set; }

        public string ClientId { get; set; }
    }
}
