using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NetCoreSse
{
    public class SseMiddleware
    {
        public Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return Task.CompletedTask;
        }
    }
}
