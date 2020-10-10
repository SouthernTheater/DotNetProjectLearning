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

            #region ��־NLOG
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

            #region CAP����
            //ע��: ע��ķ�����Ҫ�� `services.AddCap()` ֮ǰ
            //services.AddTransient<IEventSubscriberService, EventSubscriberService>();
            //services.AddSingleton<IEventSubscriberService, EventSubscriberService>();

            //services.AddDbContext<AppDbContext>();
            services.AddCap(x =>
            {
                //�����ʹ�õ� EF �������ݲ���������Ҫ����������ã�
                //x.UseEntityFramework<AppDbContext>();  //��ѡ��㲻��Ҫ�ٴ����� x.UseSqlServer ��
                var ConnString = Configuration.GetSection("ConnectionStrings:Default").Value;
                //�����ʹ�õ�ADO.NET���������ݿ�ѡ��������ã�
                x.UseSqlServer(ConnString);
                /*
                x.UseSqlServer(z =>
                {
                    z.ConnectionString = ConnString;
                    //z.Schema = "";
                });
                */

                //CAP֧�� RabbitMQ��Kafka��AzureServiceBus ����ΪMQ������ʹ��ѡ�����ã�
                var rabbitMqClientOptions = new RabbitMQClientOptions();
                Configuration.GetSection("RabbitMQClient").Bind(rabbitMqClientOptions);
                x.UseRabbitMQ(z =>
                {
                    z.VirtualHost = rabbitMqClientOptions.VirtualHost;
                    z.HostName = rabbitMqClientOptions.Host;
                    z.Port = rabbitMqClientOptions.Port;
                    z.UserName = rabbitMqClientOptions.Username;
                    z.Password = rabbitMqClientOptions.Password;
                    //��������
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
                //Ĭ�϶�����
                x.DefaultGroup = GlobalConsts.CapDefaultGroup;
                ;
            });

            #endregion

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //ʹ��NLog��Ϊ��־��¼����
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
            #region ע������
            containerBuilder.RegisterDynamicProxy();//ע��AOP��̬����Ŀǰʹ��AspectCore
            //containerBuilder.Populate(services);//�ܵ��ľ�
            //builder.RegisterType<AutofaceTest.Service.Service.UserService>().As<Service.Interface.IUserService>();//UserServiceע�뵽IUserService
            //ģ�黯ע�룬Ĭ��ע��ģ��
            containerBuilder.RegisterModule<DefaultRegisterModule>();
            //ApplicationContainer = containerBuilder.Build();//IUserService UserService ����
            //return new AutofacServiceProvider(ApplicationContainer);//��autofac�������ܵ���
            #endregion
        }
    }
}
