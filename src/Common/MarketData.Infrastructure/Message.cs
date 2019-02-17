using System;

namespace MarketData.Common.Infrastructure
{
    // See code from https://github.com/aspnet/SignalR/blob/eee2683b5ede1f10d85e78f29c11c4d8f70ef330/src/Microsoft.AspNetCore.Sockets.Http/Transports/WebSocketsTransport.cs#L124-L183
    public struct Message
    {
        public bool EndOfMessage { get; }
        public MessageType Type { get; }

        public byte[] Payload { get; }

        public Message(byte[] payload, MessageType type)
            : this(payload, type, endOfMessage: true)
        {

        }

        public Message(byte[] payload, MessageType type, bool endOfMessage)
        {
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            Type = type;
            EndOfMessage = endOfMessage;
        }
    }
}