using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Tubumu.Meeting.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("serilog.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"serilog.{environment}.json", optional: true, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(config)
                        .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog(dispose: true)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var configs = new ConfigurationBuilder()
                    .AddJsonFile("hosting.json", optional: true)
                    .AddJsonFile($"hosting.{environment}.json", optional: true)
                    .AddJsonFile("mediasoupsettings.json", optional: false)
                    .AddJsonFile($"mediasoupsettings.{environment}.json", optional: true)
                    .AddJsonFile("sipsettings.json", optional: false)
                    .AddJsonFile($"sipsettings.{environment}.json", optional: true)
                    .Build();

                webBuilder.UseConfiguration(configs);
                webBuilder.UseStartup<Startup>();
            });
    }
}
