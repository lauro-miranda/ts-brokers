using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;

namespace TS.Brokers.SiloHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
             .UseOrleans((ctx, builder) =>
             {
                 builder.UseLocalhostClustering();
                 builder.AddMemoryGrainStorage("customerStore");
                 builder.AddMemoryGrainStorage("dayTradeStore");
                 builder.AddMemoryGrainStorage("swingTradeStore");
                 builder.AddMemoryGrainStorage("balanceStore");

                 builder.AddSimpleMessageStreamProvider("stock-stream-provider")
                    .AddMemoryGrainStorage("PubSubStore");

                 builder.UseDashboard(options =>
                 {
                     options.Port = 8888;
                 });
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