using System.Threading.Tasks;
using AspNetCoreRateLimitAbpDemo.Models.TokenAuth;
using AspNetCoreRateLimitAbpDemo.Web.Controllers;
using Shouldly;
using Xunit;

namespace AspNetCoreRateLimitAbpDemo.Web.Tests.Controllers
{
    public class HomeController_Tests: AspNetCoreRateLimitAbpDemoWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}