using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AspectCore.Extensions.Autofac;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DoJob.Jobs;
using Hangfire;
using Hangfire.Console;
using Hangfire.Redis;
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

        /*
        private static BackgroundJobServer _backgroundJobServer;

        private static readonly BackgroundJobServerOptions JobServerOptions =
            new BackgroundJobServerOptions()
            {
                WorkerCount = 1,
                //队列名要小写
                Queues = new[] {"default","news"},
                ServerName = Environment.MachineName + "App1"
            };
        */

        private static List<BackgroundJobServer> _backgroundJobServerList=new List<BackgroundJobServer>();
        private static readonly List<BackgroundJobServerOptions> JobServerOptionsList =
            new List<BackgroundJobServerOptions>
            {
                new BackgroundJobServerOptions()
                {
                    //ServerName = $"{Environment.MachineName}}",
                    WorkerCount = 1,
                    //队列名要小写
                    Queues = new[] { "default"},
                    ServerName = Environment.MachineName + "App1"
                },
                new BackgroundJobServerOptions()
                {
                    //ServerName = $"{Environment.MachineName}}",
                    WorkerCount = 1,
                    //队列名要小写
                    Queues = new[] {"news"},
                    ServerName = Environment.MachineName + "App1"
                }
            };

        private static IGlobalConfiguration<RedisStorage> _redisStorage;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            SetConfiguration();
            ServiceCollections = new ServiceCollection();

            ConfigureServices(ServiceCollections);
            ILogger<Program> logger = ServiceProvider.GetService<ILogger<Program>>();
            logger.LogInformation("启动服务");
            try
            {
                //var doPublishEventTestService = ServiceProvider.GetService<DoPublishEventTestService>();
                //await doPublishEventTestService.OnStart();
                //Console.WriteLine("123");

                //使用Autofac必须再全局配置GlobalConfiguration.Configuration.UseRedisStorage后面启用,
                //且创建BackgroundJobServer必须放在UseAutofacActivator方法后面，不然无效。
                // 部分问题：BackgroundJobServer以全局实例注入后，无法获取到
                _redisStorage.UseAutofacActivator(ApplicationContainer);
                //ServiceCollections.AddSingleton(redisStorage);

                foreach (var backgroundJobServerOption in JobServerOptionsList)
                {
                    //如要按队列来分配后台作业服务器，则可创建多个后台作业服务器实例
                    var backgroundJobServer = new BackgroundJobServer(backgroundJobServerOption, _redisStorage.Entry);
                    _backgroundJobServerList.Add(backgroundJobServer);
                }


                //_backgroundJobServer ??= new BackgroundJobServer(JobServerOptions);
                // Autofac接管前可用
                //ServiceCollections.AddSingleton(_backgroundJobServer);
                // Autofac接管后需要用Aufofac的注入
                //AppContainerBuilder.RegisterInstance(_backgroundJobServer).SingleInstance();
                //AppContainerBuilder.RegisterInstance<BackgroundJobServer>(_backgroundJobServer).SingleInstance();

                //BackgroundJob.Enqueue<TestJob>(x => x.CallUrl("https://www.qq.com", "ConsoleServerJob"));
                //BackgroundJob.Enqueue<GetNewsJob>(x => x.GetNewsByUrl("https://www.qq.com", "获取腾讯新闻"));

                var backgroundJobServer2 = ServiceProvider.GetService<BackgroundJobServer>();
                var backgroundJobClient = ServiceProvider.GetService<IBackgroundJobClient>();
                //注意，执行job方法里 的PerformContext参数必须填null
                backgroundJobClient.Enqueue<GetNewsJob>(t =>
                    t.GetBaiduNews("获取百度新闻1111123", null));
            }
            catch (Exception ex)
            {
                Console.WriteLine("启动出错");
                logger.LogError("启动报错：" + ex);
                /*
                if (_backgroundJobServer != null)
                {
                    await _backgroundJobServer.WaitForShutdownAsync(cancellationToken);
                    _backgroundJobServer.SendStop();
                    _backgroundJobServer.Dispose();
                }
                */
                if (_backgroundJobServerList.Count>1)
                {
                    foreach (var backgroundJobServer in _backgroundJobServerList)
                    {
                        await backgroundJobServer.WaitForShutdownAsync(cancellationToken);
                        backgroundJobServer.SendStop();
                        backgroundJobServer.Dispose();
                    }
                }
                Console.ReadKey();
            }
            //return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            /*
            if (_backgroundJobServer != null)
            {
                await _backgroundJobServer.WaitForShutdownAsync(cancellationToken);
                _backgroundJobServer.SendStop();
                _backgroundJobServer.Dispose();
            }
            */

            if (_backgroundJobServerList.Count > 1)
            {
                foreach (var backgroundJobServer in _backgroundJobServerList)
                {
                    await backgroundJobServer.WaitForShutdownAsync(cancellationToken);
                    backgroundJobServer.SendStop();
                    backgroundJobServer.Dispose();
                }
            }
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

            #region Hangfire配置

            //hangfire配置参考：
            //https://docs.hangfire.io/en/latest/background-processing/processing-jobs-in-console-app.html
            var redisStorageOptions = new RedisStorageOptions();
            redisStorageOptions.Db = 0;
            //任务过期检查频率
            redisStorageOptions.ExpiryCheckInterval = TimeSpan.FromHours(1);
            redisStorageOptions.DeletedListSize = 10000;
            redisStorageOptions.SucceededListSize = 350000;
            if (_redisStorage == null)
            {
                _redisStorage =
                    GlobalConfiguration.Configuration.UseRedisStorage(Configuration["HangfireConnection"],
                        redisStorageOptions);
                _redisStorage.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseConsole();
            }
            

            #endregion

            //services.AddHttpClient();
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