using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    /*webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        // Set properties and call methods on options
                    });*/
                    webBuilder.UseStartup<Startup>().UseUrls("http://localhost:5005");
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory());//使用AutoFac
    }
}
