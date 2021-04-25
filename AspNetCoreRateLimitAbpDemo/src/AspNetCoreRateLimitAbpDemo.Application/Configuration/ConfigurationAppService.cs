using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using AspNetCoreRateLimitAbpDemo.Configuration.Dto;

namespace AspNetCoreRateLimitAbpDemo.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : AspNetCoreRateLimitAbpDemoAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
