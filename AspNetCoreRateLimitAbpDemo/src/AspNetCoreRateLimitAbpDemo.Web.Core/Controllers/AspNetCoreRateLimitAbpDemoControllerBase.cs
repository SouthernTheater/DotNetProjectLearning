using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreRateLimitAbpDemo.Controllers
{
    public abstract class AspNetCoreRateLimitAbpDemoControllerBase: AbpController
    {
        protected AspNetCoreRateLimitAbpDemoControllerBase()
        {
            LocalizationSourceName = AspNetCoreRateLimitAbpDemoConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
