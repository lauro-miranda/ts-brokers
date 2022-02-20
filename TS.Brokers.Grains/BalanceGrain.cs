using Orleans;
using Orleans.Providers;
using System.Threading.Tasks;
using TS.Brokers.GrainInterfaces;
using TS.Brokers.Messages.Balances;
using TS.Brokers.States;

namespace TS.Brokers.Grains
{
    [StorageProvider(ProviderName = "balanceStore")]
    public class BalanceGrain : Grain<BalanceState>, IBalanceGrain
    {
        public Task Deposit(BalanceRequestMessage message)
        {
            if (!string.IsNullOrEmpty(State.Identification))
                return Task.FromResult(Deposit(message.Value));

            State = new BalanceState { Identification = this.GetPrimaryKeyString(), Value = message.Value };
            return Task.CompletedTask;
        }

        public Task Deposit(decimal value)
        {
            return Task.FromResult(State.Value += value);
        }

        public Task<BalanceState> Get() => Task.FromResult(State);

        public Task Update(decimal value) => Task.FromResult(State.Value = value);
    }
}