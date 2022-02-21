using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Orleans;
using Orleans.Hosting;
using System.Collections.Concurrent;
using System.Linq;
using TS.Brokers.Api.Services;
using TS.Brokers.Api.SSE;
using TS.Brokers.Api.SSE.Interfaces;
using TS.Brokers.Api.SSE.Services;
using TS.Brokers.Api.SSE.Services.Interfaces;
using TS.Brokers.States;

namespace TS.Brokers.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddServerSentEvents();

            services.AddServerSentEvents<IStockExchangeServerSentEvents, StockExchangeServerSentEvents>(options => 
            {
                options.ReconnectInterval = 5000;
            });

            services.AddTransient<IStockExchangeNotificationService, StockExchangeService>();

            services.AddResponseCompression(options => 
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "text/event-stream" });
            });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TS.Brokers.Api", Version = "v1" });
            });

            CreateClusterClient(services);

            services.AddSingleton<ConcurrentDictionary<string, StockState>>()
                .AddSingleton<ConcurrentDictionary<string, BalanceState>>();

            services.AddHostedService<StockBackgroundService>();
            services.AddHostedService<BalanceBackgroundService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            app.UseResponseCompression();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Laboratório: Corretora"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapServerSentEvents<StockExchangeServerSentEvents>("stock-notifications");
                endpoints.MapControllers();
            });
        }

        void CreateClusterClient(IServiceCollection services)
        {
            var builder = new ClientBuilder();

            builder.UseLocalhostClustering();

            builder.AddSimpleMessageStreamProvider("stock-stream-provider");
            builder.AddSimpleMessageStreamProvider("balance-stream-provider");

            var client = builder.Build();

            client.Connect().GetAwaiter().GetResult();

            services.AddSingleton(client);
        }
    }
}