using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;

namespace TS.Brokers.Api
{
    public class Program
    {
        static string RedisConnection { get; } = "127.0.1.1:6379,password=12345678";

        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
             .UseOrleans((ctx, builder) =>
             {
                 if (ctx.HostingEnvironment.IsDevelopment())
                 {
                     builder.UseLocalhostClustering();
                     builder.AddRedisGrainStorage("customerStore", options =>
                     {
                         options.ConnectionString = RedisConnection;
                         options.UseJson = true;
                         options.DatabaseNumber = 1;
                     });
                 }
                 else
                 {
                     builder.UseKubernetesHosting();
                 }
             })
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder.UseStartup<Startup>();
             })
             .RunConsoleAsync()
             .GetAwaiter()
             .GetResult();
        }
    }
}