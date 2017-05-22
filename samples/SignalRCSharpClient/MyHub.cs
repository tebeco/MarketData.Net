using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRCSharpClient
{
    public class MyHub : Hub
    {
        public MyHub()
        {
            Task.Run(async () =>
            {
                Thread.Sleep(4000);
                await Send("smndbfskjhdfjkh");
            });
        }

        public async Task Send(string data)
        {
            try
            {
                await Clients.All.InvokeAsync("send", data);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        public override Task OnConnectedAsync()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("connected");
            Console.ResetColor();

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("disconnected");
            Console.ResetColor();

            return base.OnDisconnectedAsync(exception);
        }
    }
}