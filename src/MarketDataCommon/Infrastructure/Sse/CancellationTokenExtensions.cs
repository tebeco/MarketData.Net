using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace NetCoreSse
{
    public static class CancellationTokenExtensions
    {
        public static CancellationTokenAwaiter GetAwaiter(this CancellationToken cancellationToken)
        {
            return new CancellationTokenAwaiter(cancellationToken);
        }

        public class CancellationTokenAwaiter : INotifyCompletion
        {
            private readonly CancellationToken _cancellationToken;

            public CancellationTokenAwaiter(CancellationToken cancellationToken)
            {
                _cancellationToken = cancellationToken;
            }

            public void GetResult()
            {
            }

            public bool IsCompleted
            {
                get
                {
                    return _cancellationToken.IsCancellationRequested;
                }
            }

            public void OnCompleted(Action action)
            {
                _cancellationToken.Register(action);
            }
        }
    }
}
