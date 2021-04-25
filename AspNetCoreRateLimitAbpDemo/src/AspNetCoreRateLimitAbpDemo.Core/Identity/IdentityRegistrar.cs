using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using AspNetCoreRateLimitAbpDemo.Authorization;
using AspNetCoreRateLimitAbpDemo.Authorization.Roles;
using AspNetCoreRateLimitAbpDemo.Authorization.Users;
using AspNetCoreRateLimitAbpDemo.Editions;
using AspNetCoreRateLimitAbpDemo.MultiTenancy;

namespace AspNetCoreRateLimitAbpDemo.Identity
{
    public static class IdentityRegistrar
    {
        public static IdentityBuilder Register(IServiceCollection services)
        {
            services.AddLogging();

            return services.AddAbpIdentity<Tenant, User, Role>()
                .AddAbpTenantManager<TenantManager>()
                .AddAbpUserManager<UserManager>()
                .AddAbpRoleManager<RoleManager>()
                .AddAbpEditionManager<EditionManager>()
                .AddAbpUserStore<UserStore>()
                .AddAbpRoleStore<RoleStore>()
                .AddAbpLogInManager<LogInManager>()
                .AddAbpSignInManager<SignInManager>()
                .AddAbpSecurityStampValidator<SecurityStampValidator>()
                .AddAbpUserClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
                .AddPermissionChecker<PermissionChecker>()
                .AddDefaultTokenProviders();
        }
    }
}
