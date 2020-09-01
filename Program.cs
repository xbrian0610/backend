using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mywebsite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => //呼叫IIS 設定Web Server
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((webHostBuilder, configurationBinder) =>
                {
                    configurationBinder.AddJsonFile("appsettings.json", optional: true);
                })
                .UseKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = 209715200;
                })
                .UseStartup<Startup>();
    }
}
