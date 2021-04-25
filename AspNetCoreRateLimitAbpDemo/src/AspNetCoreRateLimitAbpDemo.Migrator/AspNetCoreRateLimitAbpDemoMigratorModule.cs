using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AspNetCoreRateLimitAbpDemo.Configuration;
using AspNetCoreRateLimitAbpDemo.EntityFrameworkCore;
using AspNetCoreRateLimitAbpDemo.Migrator.DependencyInjection;

namespace AspNetCoreRateLimitAbpDemo.Migrator
{
    [DependsOn(typeof(AspNetCoreRateLimitAbpDemoEntityFrameworkModule))]
    public class AspNetCoreRateLimitAbpDemoMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public AspNetCoreRateLimitAbpDemoMigratorModule(AspNetCoreRateLimitAbpDemoEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(AspNetCoreRateLimitAbpDemoMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                AspNetCoreRateLimitAbpDemoConsts.ConnectionStringName
            );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus), 
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AspNetCoreRateLimitAbpDemoMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
