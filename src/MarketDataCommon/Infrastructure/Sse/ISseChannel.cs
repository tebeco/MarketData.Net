using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreSse
{
    public interface ISseChannel : IDisposable
    {
        Task SendAsync(ServerSentEvent message, CancellationToken token);
    }
}
 
