using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TS.Brokers.GrainInterfaces;
using TS.Brokers.States;

namespace TS.Brokers.Api.Services
{
    public class BalanceBackgroundService : BackgroundService
    {
        IClusterClient ClusterClient { get; }

        ConcurrentDictionary<string, BalanceState> Balances { get; set; }

        Guid Id { get; } = Guid.NewGuid();

        List<string> CPFs = new List<string>
        {
            "05924282732", "14888280754"
        };

        public BalanceBackgroundService(IClusterClient clusterClient
                , ConcurrentDictionary<string, BalanceState> stock)
        {
            ClusterClient = clusterClient;
            Balances = stock;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var cpf in CPFs)
            {
                var grain = ClusterClient.GetGrain<IBalanceGrain>(cpf);

                await grain.SubscribeAsync(Id, "balance-namespace");

                var stream = ClusterClient.GetStreamProvider("balance-stream-provider")
                    .GetStream<BalanceState>(Id, "balance-namespace");

                await stream.SubscribeAsync(OnNextAsync);
            }
        }

        Task OnNextAsync(BalanceState state, StreamSequenceToken token = null)
        {
            Balances[state.Identification] = state;
            return Task.CompletedTask;
        }
    }
}