using Orleans;
using Orleans.Providers;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TS.Brokers.GrainInterfaces;
using TS.Brokers.States;

namespace TS.Brokers.Grains
{
    [StorageProvider(ProviderName = "PubSubStore")]
    public class StockExchangeGrain : Grain<StockState>, IStockExchangeGrain
    {
        IAsyncStream<StockState> Stream { get; set; }

        IDisposable Timer { get; set; }

        public override async Task OnActivateAsync()
        {
            await ReadStateAsync();

            State = new StockState
            {
                Symbol = this.GetPrimaryKeyString(),
                Price = 27.00m,
                Quantity = 1000
            };

            await base.OnActivateAsync();
        }

        public Task SubscribeAsync(Guid id, string namespaceName)
        {
            Stream = GetStreamProvider("stock-stream-provider").GetStream<StockState>(id, namespaceName);

            var period = TimeSpan.FromSeconds(1);
            Timer = RegisterTimer(TimerTick, null, period, period);

            return Task.CompletedTask;
        }

        async Task TimerTick(object _)
        {
            State.Price = Convert.ToDecimal($"{new Random().Next(20, 30)},{new Random().Next(101)}");
            await Stream.OnNextAsync(State);
        }

        public Task StopProducing()
        {
            if (Stream != null)
            {
                Timer.Dispose();
                Timer = null;
                Stream = null;
            }

            return Task.CompletedTask;
        }

        public Task<decimal> GetPrice()
        {
            return Task.FromResult(State.Price);
        }
    }
}