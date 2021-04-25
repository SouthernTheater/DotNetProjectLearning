using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using AspNetCoreRateLimitAbpDemo.EntityFrameworkCore.Seed;

namespace AspNetCoreRateLimitAbpDemo.EntityFrameworkCore
{
    [DependsOn(
        typeof(AspNetCoreRateLimitAbpDemoCoreModule), 
        typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class AspNetCoreRateLimitAbpDemoEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<AspNetCoreRateLimitAbpDemoDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        AspNetCoreRateLimitAbpDemoDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        AspNetCoreRateLimitAbpDemoDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AspNetCoreRateLimitAbpDemoEntityFrameworkModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
