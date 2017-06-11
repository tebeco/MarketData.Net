namespace MarketData.Infrastructure
{
    // See code from https://github.com/aspnet/SignalR/blob/eee2683b5ede1f10d85e78f29c11c4d8f70ef330/src/Microsoft.AspNetCore.Sockets.Http/Transports/WebSocketsTransport.cs#L124-L183

    public enum MessageType
    {
        Text,
        Binary,
        Close,
        Error
    }
}