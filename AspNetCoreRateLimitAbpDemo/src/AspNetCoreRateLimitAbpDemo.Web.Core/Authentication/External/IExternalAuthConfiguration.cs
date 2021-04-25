using System.Collections.Generic;

namespace AspNetCoreRateLimitAbpDemo.Authentication.External
{
    public interface IExternalAuthConfiguration
    {
        List<ExternalLoginProviderInfo> Providers { get; }
    }
}
