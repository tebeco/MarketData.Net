using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreSse
{
    public class MulticastChannel : ISseChannel
    {
        private readonly IList<ISseChannel> _channels = new List<ISseChannel>();
        private readonly int _replayBufferSize;
        private readonly IList<ServerSentEvent> _replayBuffer;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private readonly object _syncRoot = new object();

        public MulticastChannel(int replayBufferSize = 1)
        {
            _replayBufferSize = replayBufferSize;
            _replayBuffer = new List<ServerSentEvent>(replayBufferSize);
        }

        public async Task AddChannel(ISseChannel channel, CancellationToken token)
        {
            try
            {
                Monitor.Enter(_syncRoot);

                _channels.Add(channel);
                foreach (var message in _replayBuffer)
                {
                    await channel.SendAsync(message, token).ConfigureAwait(false);
                }
            }
            finally
            {
                Monitor.Exit(_syncRoot);
            }
        }

        public async Task SendAsync(ServerSentEvent message, CancellationToken token)
        {
            try
            {
                Monitor.Enter(_syncRoot);

                var closeChannels = new List<ISseChannel>();
                foreach (var channel in _channels)
                {
                    try
                    {
                        await channel.SendAsync(message, token).ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        closeChannels.Add(channel);
                    }
                }
                foreach (var channel in closeChannels)
                {
                    _channels.Remove(channel);
                }

                while (_replayBuffer.Count >= _replayBufferSize && _replayBuffer.Count > 0)
                {
                    _replayBuffer.RemoveAt(0);
                }
                _replayBuffer.Add(message);
            }
            finally
            {
                Monitor.Exit(_syncRoot);
            }
        }

        public void AttachDisposable(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        public void Dispose()
        {
            foreach (var channel in _channels)
            {
                channel.Dispose();
            }
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
