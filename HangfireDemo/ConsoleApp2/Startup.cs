using System;
using System.Net.Http;
using System.Threading.Tasks;
using AspectCore.Extensions.Autofac;
using Autofac;
using DoJob.Jobs;
using Hangfire;
using Hangfire.Console;
using Hangfire.Redis;
using Hangfire.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace ConsoleApp2
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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

            #region Hangfire配置
            // Add Hangfire services.
            var redisStorageOptions = new RedisStorageOptions();
            redisStorageOptions.Db = 0;
            //任务过期检查频率
            redisStorageOptions.ExpiryCheckInterval = TimeSpan.FromHours(1);
            //redisStorageOptions.Prefix = null;
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseRedisStorage(Configuration["HangfireConnection"], redisStorageOptions)
                .UseConsole()
            );

            // Add the processing server as IHostedService
            //Configure方法中与则该方法可以不调用
            services.AddHangfireServer(opt =>
            {
                opt.WorkerCount = 1;
                //队列名要小写
                opt.Queues = new[] { "news", "default" };
                opt.ServerName = Environment.MachineName + "App2";
            });
            //services.AddTransient<PerformContext>();
            
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            IBackgroundJobClient backgroundJobs,
            ILoggerFactory loggerFactory)
        {
            #region 启用Hangfire
            app.UseHangfireDashboard();
            //ConfigureServices启用了Server则不用再启用
            /*
            var jobServerOptions = new BackgroundJobServerOptions
            {
                //ServerName = $"{Environment.MachineName}}",
                WorkerCount = 2,
                Queues = new[] { "news", "default" },
                ServerName = Environment.MachineName + "App2"
            };
            app.UseHangfireServer(jobServerOptions);
            */

            //backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
            //backgroundJobs.Create()
            /*
            BackgroundJob.Schedule(
                () => Console.WriteLine("你好 ConsoleApp2！"),
                TimeSpan.FromSeconds(20));
            */
            //RecurringJob.AddOrUpdate();
            //var a=new BackgroundJobClient();
            //a.Create(new Job(), )

            //BackgroundJob.Enqueue<TestJob>(x => x.CallUrl("https://www.qq.com", "ConsoleApp2"));

            //BackgroundJob.Enqueue<GetNewsJob>(x => x.GetNewsByUrl("https://www.qq.com", "获取腾讯新闻"));
            //BackgroundJob.Enqueue<GetNewsJob>(x => x.GetNewsByUrl("https://www.baidu.com/", "获取百度新闻"));
            /*
            BackgroundJob.Schedule<GetNewsJob>(t => 
                t.GetNewsByUrl("https://www.baidu.com","延迟获取百度新闻"),
                TimeSpan.FromSeconds(10));
            */
            //var performContext = app.ApplicationServices.GetService<PerformContext>();
            var redisStorage = app.ApplicationServices.GetService<RedisStorage>();
            //var performContext=new PerformContext(redisStorage,);

            /*
            BackgroundJob.Schedule<GetNewsJob>(t => 
                t.GetBaiduNews("延迟获取百度新闻"),
                TimeSpan.FromSeconds(10));
            */
            /*
            BackgroundJob.Enqueue<GetNewsJob>(t => 
                t.GetBaiduNews("获取百度新闻22"));
            */
            var backgroundJobClient = app.ApplicationServices.GetService<IBackgroundJobClient>();
            //注意，执行job方法里 的PerformContext参数必须填null
            backgroundJobClient.Enqueue<GetNewsJob>(t => 
                t.GetBaiduNews("获取百度新闻444",null));

            #endregion

        }

        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            #region 注入容器
            containerBuilder.RegisterDynamicProxy();//注册AOP动态代理，目前使用AspectCore
            //containerBuilder.Populate(services);//管道寄居
            //builder.RegisterType<AutofaceTest.Service.Service.UserService>().As<Service.Interface.IUserService>();//UserService注入到IUserService
            //模块化注入，默认注入模块
            containerBuilder.RegisterModule<DefaultRegisterModule>();
            //ApplicationContainer = containerBuilder.Build();//IUserService UserService 构造
            //return new AutofacServiceProvider(ApplicationContainer);//将autofac反馈到管道中

            //GlobalConfiguration.Configuration.UseAutofacActivator(containerBuilder.Build());
            #endregion
        }

    }
}