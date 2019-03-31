using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NewMao.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                var env = hostContext.HostingEnvironment;
                config.SetBasePath(env.ContentRootPath)
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(path: $"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);
            })
            .UseStartup<Startup>()
            .UseKestrel(options =>
            {
                options.Listen(IPAddress.Loopback, 5070);
            });
            //WebHost.CreateDefaultBuilder(args)
            //    .UseStartup<Startup>();
    }
}
