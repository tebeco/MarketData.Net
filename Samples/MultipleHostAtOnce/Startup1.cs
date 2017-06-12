using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MultipleHostAtOnce
{
    public class Startup1
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        private void OnApplicationStarted(ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<Startup1>();
            logger.LogDebug("Application 1 started");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            loggerFactory.AddConsole()
                         .AddDebug();
            applicationLifetime.ApplicationStarted.Register(() =>
            {
                var localLoggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
                OnApplicationStarted(localLoggerFactory);
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World from Host 1 !");
            });
        }
    }
}
