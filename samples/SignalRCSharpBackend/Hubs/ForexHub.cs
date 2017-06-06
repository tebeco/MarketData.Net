using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Internal.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRCSharpBackend
{
    public class ForexHub: Hub
    {
        public IEnumerable<int> Stream(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return i;
            }
        }
    }
}
