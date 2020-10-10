using DotNetCore.CAP.Dashboard;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class CapDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            //访问地址：http://localhost:49391/cap?accesskey=654321
            if (context.Request.GetQuery("accesskey") == "654321")
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AuthorizeAsync(DashboardContext context)
        {
            //访问地址：http://localhost:49391/cap?accesskey=654321
            if (context.Request.GetQuery("accesskey") == "654321")
            {
                return true;
            }
            return false;
        }
    }
}