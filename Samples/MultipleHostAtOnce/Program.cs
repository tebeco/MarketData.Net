using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

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
