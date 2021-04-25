using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreRateLimitAbpDemo.EntityFrameworkCore
{
    public static class AspNetCoreRateLimitAbpDemoDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AspNetCoreRateLimitAbpDemoDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<AspNetCoreRateLimitAbpDemoDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
