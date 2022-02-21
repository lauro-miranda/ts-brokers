using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TS.Brokers.Api.SSE.Services.Interfaces;
using TS.Brokers.GrainInterfaces;
using TS.Brokers.Messages.StockExchanges.SSE;
using TS.Brokers.States;

namespace TS.Brokers.Api.Services
{
    public class StockBackgroundService : BackgroundService
    {
        IClusterClient ClusterClient { get; }

        ConcurrentDictionary<string, StockState> Stock { get; set; }

        IStockExchangeNotificationService StockExchangeNotificationService { get; }

        Guid Id { get; } = Guid.NewGuid();

        List<string> Stocks = new List<string>
        {
            "PETR1", "PETR2", "PETR3", "PETR4", "PETR5"
        };

        public StockBackgroundService(IClusterClient clusterClient
            , ConcurrentDictionary<string, StockState> stock
            , IStockExchangeNotificationService stockExchangeNotificationService)
        {
            ClusterClient = clusterClient;
            Stock = stock;
            StockExchangeNotificationService = stockExchangeNotificationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var stock in Stocks)
            {
                var grain = ClusterClient.GetGrain<IStockExchangeGrain>(stock);

                await grain.SubscribeAsync(Id, "stock-namespace");

                var stream = ClusterClient.GetStreamProvider("stock-stream-provider")
                    .GetStream<StockState>(Id, "stock-namespace");

                await stream.SubscribeAsync(OnNextAsync);
            }
        }

        async Task OnNextAsync(StockState state, StreamSequenceToken token = null)
        {
            Stock[state.Symbol] = state;
            await StockExchangeNotificationService.SendAsync(new StockUpdatedMessage
            {
                Price = state.Price,
                Quantity = state.Quantity,
                Symbol = state.Symbol
            });
        }
    }
}