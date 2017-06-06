using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRCSharpClient
{

    public class User
    {
        public string Name { get; set; }
    }

    public class Message
    {
        public User User { get; set; }
        public string id { get; set; }
        public string MessageContent { get; set; }
    }
    public class UserJoinedChannel
    {
        public User User { get; set; }
    }

    public class UserLeftChannel
    {
        public User User { get; set; }
    }

    public class UserSentMessage
    {
        public User User { get; set; }
        public Message Message { get; set; }
    }

    public class ChatHub : Hub
    {
        public async Task UserSentMessage(User user, Message message)
        {
            var userSentMessage = new UserSentMessage
            {
                User = user,
                Message = message
            };

            await SendPayload("userSentMessage", userSentMessage);
        }

        private static JsonSerializerSettings lowerCamelCaseSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        public async Task SendPayload(string methodName, object payload)
        {
            try
            {
                var stringPayload = JsonConvert.SerializeObject(payload, lowerCamelCaseSettings);

                Console.WriteLine(stringPayload);

                await Clients.All.InvokeAsync(methodName, stringPayload);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        public async override Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var currentUser = new User
            {
                Name = this.Context.ConnectionId
            };

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"New user connected : {currentUser.Name}");
            Console.ResetColor();

            var payload = new UserJoinedChannel
            {
                User = currentUser,
            };

            await SendPayload("userJoinedChannel", payload);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);

            var currentUser = new User
            {
                Name = this.Context.ConnectionId
            };

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"User disconnected : {currentUser.Name}");
            Console.ResetColor();

            var payload = new UserLeftChannel
            {
                User = currentUser,
            };

            await SendPayload("userLeftChannel", payload);
        }
    }
}



