using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AspectCore.Extensions.Autofac;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ConsoleApp1.DoService;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace ConsoleApp1
{
    public class AppHostedService : IHostedService
    {

        private static IServiceProvider ServiceProvider { get; set; }
        private static IContainer ApplicationContainer { get; set; }
        private static IServiceCollection ServiceCollections { get; set; }

        private static IConfiguration Configuration { get; set; }

        private static ContainerBuilder AppContainerBuilder { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            SetConfiguration();
            ServiceCollections = new ServiceCollection();

            ConfigureServices(ServiceCollections);
            ILogger<Program> logger = ServiceProvider.GetService<ILogger<Program>>();
            logger.LogInformation("启动服务");
            try
            {
                var doTestService = ServiceProvider.GetService<DoTestService>();
                await doTestService.OnStart();
            }
            catch (Exception ex)
            {
                Console.WriteLine("启动出错");
                logger.LogError("启动报错：" + ex);
                Console.ReadKey();
            }
            //return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }


        static void SetConfiguration()
        {
            #region 配置文件

            //在当前目录或者根目录中寻找appsettings.json文件
            var fileName = "appsettings.json";

            var directory = AppContext.BaseDirectory;
            directory = directory.Replace("\\", "/");

            var filePath = $"{directory}/{fileName}";
            if (!File.Exists(filePath))
            {
                var length = directory.IndexOf("/bin");
                filePath = $"{directory.Substring(0, length)}/{fileName}";
            }

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile(filePath, false, true);
            Configuration = configurationBuilder.Build();
            #endregion
        }


        static void ConfigureServices(IServiceCollection services)
        {
            //程序集注入
            services.AddMediatR(typeof(Program).Assembly);

            #region 日志NLOG
            /*var nLogConfig = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            LogManager.Configuration = new NLogLoggingConfiguration(nLogConfig.GetSection("NLog"));*/
            NLog.LogManager.LoadConfiguration("NLog.Config");
            //loggerFactory.AddNLog();
            services.AddLogging(loggingBuilder =>
            {
                // configure Logging with NLog
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                //loggingBuilder.AddNLog(nLogConfig);
                loggingBuilder.AddNLog();
            });
            #endregion


            #region 注入容器
            var containerBuilder = new ContainerBuilder();//实例化 AutoFac  容器
            containerBuilder.RegisterDynamicProxy();//注册AOP动态代理，目前使用AspectCore//模块化注入，默认注入模块
            containerBuilder.Populate(services);//管道寄居
            containerBuilder.RegisterModule<DefaultRegisterModule>();
            AppContainerBuilder = containerBuilder;

            ApplicationContainer = containerBuilder.Build();//IUserService UserService 构造 
            #endregion

            //ServiceProvider = services.BuildServiceProvider();
            ServiceProvider = new AutofacServiceProvider(ApplicationContainer);//将autofac反馈到管道中

            //ServiceProvider.GetService<IBootstrapper>().BootstrapAsync(default);

            //GlobalConfiguration.Configuration.UseAutofacActivator(ApplicationContainer);

        }

    }
}