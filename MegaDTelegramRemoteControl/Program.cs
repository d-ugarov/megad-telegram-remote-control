using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Web;
using System;
using System.IO;

namespace MegaDTelegramRemoteControl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
            
            try
            {
                logger.Info($"[Main:{Constants.InstanceId}] Application started");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                logger.Fatal($"Application crashed: {e.Message}");
                logger.Error($"[Main:{Constants.InstanceId}] Application crashed: {e.Message} {Environment.NewLine}{e.StackTrace}");
                throw;
            }
            finally
            {
                logger.Info($"[Main:{Constants.InstanceId}] Application stopped");
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
                                                                           .UseContentRoot(Directory.GetCurrentDirectory())
                                                                           .ConfigureWebHostDefaults(webBuilder =>
                                                                           {
                                                                               webBuilder.UseStartup<Startup>();
                                                                           })
                                                                           .UseNLog();
    }
}
