using System;
using System.Reactive.Concurrency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MarketDataCommon.Infrastructure
{
    public abstract class RxServerEventBroadcasterBase<T> : ICreateWebHost
    {
        private IObservable<T> _events;

        public RxServerEventBroadcasterBase(int port, bool flaky)
        {
            Port = port;
            Flaky = flaky;
        }

        public IWebHost CreateServer()
        {
            if (Flaky)
            {
                _events = SubscriptionLimiter.LimitSubscriptions(1, InitializeEventStream());
            }
            else
            {
                _events = InitializeEventStream();
            }
            return InternalCreateServer();
        }

        private IWebHost InternalCreateServer()
        {

            var host = new WebHostBuilder()
                .UseKestrel()
                //.UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                //.UseStartup<RxServerEventBroadcasterBase<T>>()
                .ConfigureServices(ConfigureServices)
                .Configure(appBuilder =>
                {
                    var env = appBuilder.ApplicationServices.GetService<IHostingEnvironment>();
                    var loggerFactory = appBuilder.ApplicationServices.GetService<ILoggerFactory>();
                    Configure(appBuilder, env, loggerFactory);
                })
                .Build();

            return host;
        }

        public IObservable<T> GetEvents(object httpRequest)
        {
            return _events;
        }

        protected abstract IObservable<T> InitializeEventStream();

        public int Port { get; }

        public bool Flaky { get; }

        #region From Startup

        public RxServerEventBroadcasterBase(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }


        #endregion
    }
}
