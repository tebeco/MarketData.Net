using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRCSharpClient
{
    public class MyHub : Hub
    {
        public Task Send(string data)
        {
             return Clients.All.InvokeAsync("Send", data);
        }
    }
}