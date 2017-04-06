using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace MarketDataExternal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var market = new Market();
            market.Start();

            Console.WriteLine("**** Providers ready! ****");
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
