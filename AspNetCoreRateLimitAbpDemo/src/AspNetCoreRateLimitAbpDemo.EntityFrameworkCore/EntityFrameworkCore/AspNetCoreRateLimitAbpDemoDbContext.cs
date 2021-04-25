using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using AspNetCoreRateLimitAbpDemo.Authorization.Roles;
using AspNetCoreRateLimitAbpDemo.Authorization.Users;
using AspNetCoreRateLimitAbpDemo.MultiTenancy;

namespace AspNetCoreRateLimitAbpDemo.EntityFrameworkCore
{
    public class AspNetCoreRateLimitAbpDemoDbContext : AbpZeroDbContext<Tenant, Role, User, AspNetCoreRateLimitAbpDemoDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public AspNetCoreRateLimitAbpDemoDbContext(DbContextOptions<AspNetCoreRateLimitAbpDemoDbContext> options)
            : base(options)
        {
        }
    }
}
