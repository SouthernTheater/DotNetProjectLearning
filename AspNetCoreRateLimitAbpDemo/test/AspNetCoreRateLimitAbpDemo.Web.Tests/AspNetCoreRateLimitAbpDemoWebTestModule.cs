using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AspNetCoreRateLimitAbpDemo.EntityFrameworkCore;
using AspNetCoreRateLimitAbpDemo.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace AspNetCoreRateLimitAbpDemo.Web.Tests
{
    [DependsOn(
        typeof(AspNetCoreRateLimitAbpDemoWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class AspNetCoreRateLimitAbpDemoWebTestModule : AbpModule
    {
        public AspNetCoreRateLimitAbpDemoWebTestModule(AspNetCoreRateLimitAbpDemoEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AspNetCoreRateLimitAbpDemoWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(AspNetCoreRateLimitAbpDemoWebMvcModule).Assembly);
        }
    }
}