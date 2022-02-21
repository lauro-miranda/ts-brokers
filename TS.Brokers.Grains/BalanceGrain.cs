using Orleans;
using Orleans.Providers;
using Orleans.Streams;
using System;
using System.Threading.Tasks;
using TS.Brokers.GrainInterfaces;
using TS.Brokers.Messages.Balances;
using TS.Brokers.States;

namespace TS.Brokers.Grains
{
    [StorageProvider(ProviderName = "balanceStore")]
    public class BalanceGrain : Grain<BalanceState>, IBalanceGrain
    {
        IAsyncStream<BalanceState> Stream { get; set; }

        public async Task Deposit(BalanceRequestMessage message)
        {
            if (!string.IsNullOrEmpty(State.Identification))
            {
                await Deposit(message.Value);
                return;
            }

            State = new BalanceState { Identification = this.GetPrimaryKeyString(), Value = message.Value };

            await SendsteamAsync();
        }

        public async Task Deposit(decimal value)
        {
            State.Value += value;
            await SendsteamAsync();
        }

        public Task<BalanceState> Get() => Task.FromResult(State);

        public async Task Update(decimal value)
        {
            State.Value = value;
            await SendsteamAsync();
        }

        public Task SubscribeAsync(Guid id, string namespaceName)
        {
            State = new BalanceState { Identification = this.GetPrimaryKeyString(), Value = 0 };

            Stream = GetStreamProvider("balance-stream-provider").GetStream<BalanceState>(id, namespaceName);

            return Task.CompletedTask;
        }

        public async Task SendsteamAsync() => await Stream.OnNextAsync(State);
    }
}