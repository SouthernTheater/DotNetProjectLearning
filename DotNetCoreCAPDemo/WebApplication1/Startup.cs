using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspectCore.Extensions.Autofac;
using Autofac;
using Common;
using DotNetCore.CAP.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using TZ.RabbitMQ.Client;

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
            services.AddHttpClient();

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
            //services.AddTransient<IEventSubscriberService, EventSubscriberService>();
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
                x.UseDashboard();
                /*
                x.UseDashboard(opt => {
                    opt.Authorization = new[] { new CapDashboardAuthorizationFilter() };
                });
                */
                //默认队列名
                x.DefaultGroup = GlobalConsts.CapDefaultGroup;
                ;
            });

            #endregion

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //使用NLog作为日志记录工具
            loggerFactory.AddNLog();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

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
