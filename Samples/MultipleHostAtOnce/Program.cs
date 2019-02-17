using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace MultipleHostAtOnce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host1 = WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://localhost:5001")
                .UseStartup<Startup1>()
                .Build();

            var host2 = WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://localhost:5002")
                .UseStartup<Startup2>()
                .Build();

            host1.Start();
            host2.Start();

            Console.ReadLine();
        }
    }
}
