using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TS.Brokers.GrainInterfaces;
using TS.Brokers.States;

namespace TS.Brokers.Api.Services
{
    public class StockBackgroundService : BackgroundService
    {
        IClusterClient ClusterClient { get; }

        ConcurrentDictionary<Guid, StockState> Stock { get; set; }

        Guid Id { get; } = Guid.NewGuid();

        public StockBackgroundService(IClusterClient clusterClient
            , ConcurrentDictionary<Guid, StockState> stock)
        {
            ClusterClient = clusterClient;
            Stock = stock;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var grain = ClusterClient.GetGrain<IStockExchangeGrain>("PETR4");
            await grain.Start(Id, "stock-namespace");

            var stream = ClusterClient.GetStreamProvider("stock-stream-provider")
                .GetStream<StockState>(Id, "stock-namespace");
            
            await stream.SubscribeAsync(OnNextAsync);
        }

        Task OnNextAsync(StockState state, StreamSequenceToken token = null)
        {
            Stock[Id] = state;
            return Task.CompletedTask;
        }
    }
}