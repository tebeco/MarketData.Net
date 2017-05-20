using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebSocketServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            app.UseWebSockets();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/jsstuff.js")
                {
                    await WriteJsWebSocketScriptAsync(context);
                }
                else if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var wsHandler = new WebSocketHandler(context);
                        await wsHandler.InvokeAsync();
                        
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await WriteHtmlStaticPageAsync(context);
                }
            });
        }

        private async Task WriteJsWebSocketScriptAsync(HttpContext context)
        {
            await context.Response.WriteAsync(
                @"
var webSocket;
$().ready(function () {
  webSocket = new WebSocket('ws://localhost:5000/ws');
  webSocket.onopen = function() {
      $('#spanStatus').text('connected');
  };    webSocket.onmessage = function(evt) {
    $('#spanStatus').text(evt.data);
  };
  webSocket.onerror = function(evt) {
    alert(evt.message);
  };
  webSocket.onclose = function() {
    $('#spanStatus').text('disconnected');
  };
  $('#btnSend').click(function() {
    if (webSocket.readyState == WebSocket.OPEN)
    {
      webSocket.send($('#textInput').val());
    }
    else
    {
      $('#spanStatus').text('Connection is closed');
    }
  });
});
");
            await context.Response.Body.FlushAsync();

        }

        private async Task WriteHtmlStaticPageAsync(HttpContext context)
        {
            context.Response.StatusCode = 200;

            await context.Response.WriteAsync(
                "<html>\n" +
                "\t<head>\n" +
                "\t\t<script src=\"https://code.jquery.com/jquery-3.2.1.min.js\"></script>\n" +
                "\t\t<script src=\"http://localhost:5000/jsstuff.js\"></script>\n" +
                "\t</head>\n" +
                "\t<body>\n" +
                "\t\t<div>\n" +
                "\t\t\t<span id=\"spanStatus\">\n" +
                "\t\t\t</span>\n" +
                "\t\t<input id=\"textInput\">\n" +
                "\t\t<input id=\"btnSend\" type=\"button\">\n" +
                "\t\t</div>\n" +
                "\t</body>\n" +
                "</html>\n");

            await context.Response.Body.FlushAsync();

        }
    }
}
