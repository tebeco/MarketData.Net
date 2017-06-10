using MarketDataCommon.Infratructure.WebSocket;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCommon.Infratructure
{
    public class RxRequestReplyClient : IProduceObservable<string>
    {
        private int _port;
        private HttpClient _httpClient;
        private string _paramName;

        public RxRequestReplyClient(int port, string paramName)
        {
            _httpClient = new HttpClient();
            _port = port;
            _paramName = paramName;
        }

        public Task<IObservable<string>> GetStream() { throw new NotImplementedException(); }

        public Task<IObservable<string>> GetStream(string parameterValue)
        {
            var url = $"http://localhost:{_port}?{_paramName}={parameterValue}";
            var observable = Observable.FromAsync(async () => await _httpClient.GetAsync(url))
                              .SelectMany(response => Observable.FromAsync(async () => await response.Content.ReadAsStringAsync()));

            return Task.FromResult(observable);
        }
    }
}
