using System.Threading.Tasks;
using AspNetCoreRateLimitAbpDemo.Configuration.Dto;

namespace AspNetCoreRateLimitAbpDemo.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
