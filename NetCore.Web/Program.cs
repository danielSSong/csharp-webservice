using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCore.Services.Data;

namespace NetCore.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
            /*
            var webHost = CreateWebHostBuilder(args).Build();

            using (var scope = webHost.Services.CreateScope())
            {
                DBFirstDBInitializer initializer = scope.ServiceProvider
                                                        .GetService<DBFirstDBInitializer>();
                int rowAffected = initializer.PlantSeedData();
            }
            webHost.Run();
            */
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(builder => builder.AddFile(options =>
                {
                    options.LogDirectory = "Logs";  //logfolder
                    options.FileName = "log-";      //logfile log-2019
                    options.FileSizeLimit = null;   //logfile size limit (10mb)
                    options.RetainedFileCountLimit = null;   //logfile limit  
                }))
                .UseStartup<Startup>();
    }
}
