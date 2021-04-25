using Abp.Authorization;
using AspNetCoreRateLimitAbpDemo.Authorization.Roles;
using AspNetCoreRateLimitAbpDemo.Authorization.Users;

namespace AspNetCoreRateLimitAbpDemo.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
