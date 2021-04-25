using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using AspNetCoreRateLimitAbpDemo.Configuration;
using AspNetCoreRateLimitAbpDemo.Web;

namespace AspNetCoreRateLimitAbpDemo.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class AspNetCoreRateLimitAbpDemoDbContextFactory : IDesignTimeDbContextFactory<AspNetCoreRateLimitAbpDemoDbContext>
    {
        public AspNetCoreRateLimitAbpDemoDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AspNetCoreRateLimitAbpDemoDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            AspNetCoreRateLimitAbpDemoDbContextConfigurer.Configure(builder, configuration.GetConnectionString(AspNetCoreRateLimitAbpDemoConsts.ConnectionStringName));

            return new AspNetCoreRateLimitAbpDemoDbContext(builder.Options);
        }
    }
}
