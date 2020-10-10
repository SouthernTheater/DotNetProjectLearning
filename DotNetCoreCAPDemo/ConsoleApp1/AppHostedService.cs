using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AspectCore.Extensions.Autofac;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using ConsoleApp1.DoService;
using ConsoleApp1.IntegrationEvent;
using DotNetCore.CAP.Internal;
using DotNetCore.CAP.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using TZ.RabbitMQ.Client;

namespace ConsoleApp1
{
    public class AppHostedService : IHostedService
    {

        private static IServiceProvider ServiceProvider { get; set; }
        private static IContainer ApplicationContainer { get; set; }
        private static IServiceCollection ServiceCollections { get; set; }

        private static IConfiguration Configuration { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            SetConfiguration();
            ServiceCollections = new ServiceCollection();

            ConfigureServices(ServiceCollections);
            ILogger<Program> logger = ServiceProvider.GetService<ILogger<Program>>();
            logger.LogInformation("启动服务");
            try
            {
                var doPublishEventTestService = ServiceProvider.GetService<DoPublishEventTestService>();
                await doPublishEventTestService.OnStart();
            }
            catch (Exception ex)
            {
                Console.WriteLine("启动出错");
                logger.LogError("启动报错：" + ex);
                Console.ReadKey();
            }
            //return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;


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
            services.AddHttpClient();
            //ILoggerFactory loggerFactory;
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

            #region CAP配置
            //注意: 注入的服务需要在 `services.AddCap()` 之前
            services.AddTransient<IEventSubscriberService, EventSubscriberService>();
            //services.AddSingleton<IEventSubscriberService, EventSubscriberService>();

            //services.AddDbContext<AppDbContext>();
            services.AddCap(x =>
            {
                //如果你使用的 EF 进行数据操作，你需要添加如下配置：
                //x.UseEntityFramework<AppDbContext>();  //可选项，你不需要再次配置 x.UseSqlServer 了
                var ConnString = Configuration.GetSection("ConnectionStrings:Default").Value;
                //如果你使用的ADO.NET，根据数据库选择进行配置：
                x.UseSqlServer(ConnString);
                /*
                x.UseSqlServer(z =>
                {
                    z.ConnectionString = ConnString;
                    //z.Schema = "";
                });
                */

                //CAP支持 RabbitMQ、Kafka、AzureServiceBus 等作为MQ，根据使用选择配置：
                var rabbitMqClientOptions = new RabbitMQClientOptions();
                Configuration.GetSection("RabbitMQClient").Bind(rabbitMqClientOptions);
                x.UseRabbitMQ(z =>
                {
                    z.VirtualHost = rabbitMqClientOptions.VirtualHost;
                    z.HostName = rabbitMqClientOptions.Host;
                    z.Port = rabbitMqClientOptions.Port;
                    z.UserName = rabbitMqClientOptions.Username;
                    z.Password = rabbitMqClientOptions.Password;
                    //交换机名
                    z.ExchangeName = GlobalConsts.CapExchangeName;
                });
                x.FailedRetryCount = 2;
                x.FailedThresholdCallback = failed =>
                {
                    var logger = failed.ServiceProvider.GetService<ILogger<Program>>();
                    logger.LogError($@"A message of type {failed.MessageType} failed after executing {x.FailedRetryCount} several times, 
                        requiring manual troubleshooting. Message name: {failed.Message.GetName()}");
                };
                x.SucceedMessageExpiredAfter = 24 * 3600;
                x.UseDashboard(z =>
                {
                    
                });
                //默认队列名
                x.DefaultGroup = GlobalConsts.CapDefaultGroup;
            });

            #endregion

            #region 注入容器
            var containerBuilder = new ContainerBuilder();//实例化 AutoFac  容器
            containerBuilder.RegisterDynamicProxy();//注册AOP动态代理，目前使用AspectCore//模块化注入，默认注入模块
            containerBuilder.Populate(services);//管道寄居
            containerBuilder.RegisterModule<DefaultRegisterModule>();

            ApplicationContainer = containerBuilder.Build();//IUserService UserService 构造 
            #endregion

            //ServiceProvider = services.BuildServiceProvider();
            ServiceProvider = new AutofacServiceProvider(ApplicationContainer);//将autofac反馈到管道中

            ServiceProvider.GetService<IBootstrapper>().BootstrapAsync(default);
        }

    }
}