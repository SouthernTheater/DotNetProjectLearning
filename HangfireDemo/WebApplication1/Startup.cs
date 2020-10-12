using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using AspectCore.Extensions.Autofac;
using Autofac;
using DoJob.Jobs;
using Hangfire;
using Hangfire.Common;
using Hangfire.Console;
using Hangfire.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace WebApplication1
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
                opt.ServerName = Environment.MachineName + "Web1";
            });

            #endregion

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IWebHostEnvironment env,
            IBackgroundJobClient backgroundJobs,
            ILoggerFactory loggerFactory)
        {
            //使用NLog作为日志记录工具
            //loggerFactory.AddNLog();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            #region 启用Hangfire
            app.UseHangfireDashboard();
            /*
            var jobServerOptions = new BackgroundJobServerOptions
            {
                //ServerName = $"{Environment.MachineName}}",
                WorkerCount = 2,
            };
            app.UseHangfireServer(jobServerOptions);
            */
            //backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
            //backgroundJobs.Create()
            /*
            BackgroundJob.Schedule(
                () => Console.WriteLine("Hello, world"),
                TimeSpan.FromSeconds(20));
            */

            //RecurringJob.AddOrUpdate();
            //var a=new BackgroundJobClient();
            //a.Create(new Job(), )

            //BackgroundJob.Enqueue<CallUrlJob>(x => x.Call("https://www.baidu.com", "A11111111"));
            //BackgroundJob.Enqueue(new CallUrlJob().Call("","aaaa"));
            //var aaa=new Job();
            //BackgroundJob.Schedule<CallUrlJob>(x => x.Call("https://www.baidu.com","A12345678"));
            //BackgroundJob.Schedule<string>()
            //Expression<Func<string, Task>> methodCall2 = methodCall;
            /*
            BackgroundJob.Schedule((a) =>
            {

            }, TimeSpan.FromSeconds(10));
            */

            backgroundJobs.Schedule<GetNewsJob>(x => 
                x.GetBaiduNews("baidu news", null), 
                TimeSpan.FromSeconds(10)
            );

            backgroundJobs.Enqueue<GetNewsJob>(x => 
                x.GetBaiduNews("baidu news", null)
            );
            
            #endregion
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
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
            #endregion
        }

    }
}
