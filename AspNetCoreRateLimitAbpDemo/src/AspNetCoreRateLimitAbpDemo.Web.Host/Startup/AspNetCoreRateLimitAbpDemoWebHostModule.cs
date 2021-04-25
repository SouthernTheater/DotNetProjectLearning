using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AspNetCoreRateLimitAbpDemo.Configuration;

namespace AspNetCoreRateLimitAbpDemo.Web.Host.Startup
{
    [DependsOn(
       typeof(AspNetCoreRateLimitAbpDemoWebCoreModule))]
    public class AspNetCoreRateLimitAbpDemoWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AspNetCoreRateLimitAbpDemoWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AspNetCoreRateLimitAbpDemoWebHostModule).GetAssembly());
        }
    }
}
