// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Sockets
{
    public class Connection : IDisposable
    {
        public string ConnectionId { get; }

        public ConnectionMetadata Metadata { get; } = new ConnectionMetadata();

        public IChannelConnection<Message> Transport { get; }

        public Connection(string id, IChannelConnection<Message> transport)
        {
            Transport = transport;
            ConnectionId = id;
        }

        public void Dispose()
        {
            Transport.Dispose();
        }
    }
}
