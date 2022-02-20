using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Orleans;
using Orleans.Hosting;
using System;
using System.Collections.Concurrent;
using TS.Brokers.Api.Services;
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
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TS.Brokers.Api", Version = "v1" });
            });

            CreateClusterClient(services);

            services.AddSingleton<ConcurrentDictionary<string, StockState>>();

            services.AddHostedService<StockBackgroundService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Laboratório: Corretora"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        void CreateClusterClient(IServiceCollection services)
        {
            var builder = new ClientBuilder();

            builder.UseLocalhostClustering();

            builder.AddSimpleMessageStreamProvider("stock-stream-provider");

            var client = builder.Build();

            client.Connect().GetAwaiter().GetResult();

            services.AddSingleton(client);
        }
    }
}