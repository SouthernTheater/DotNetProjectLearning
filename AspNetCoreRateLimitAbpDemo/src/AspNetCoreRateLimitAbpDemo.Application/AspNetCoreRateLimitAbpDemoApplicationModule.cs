using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AspNetCoreRateLimitAbpDemo.Authorization;

namespace AspNetCoreRateLimitAbpDemo
{
    [DependsOn(
        typeof(AspNetCoreRateLimitAbpDemoCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class AspNetCoreRateLimitAbpDemoApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<AspNetCoreRateLimitAbpDemoAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(AspNetCoreRateLimitAbpDemoApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
