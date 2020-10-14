using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsoleApp1
{

    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder().RunConsoleAsync();
            Console.ReadKey();
        }


        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<AppHostedService>();
                });

    }
}