using Orleans;
using TS.Brokers.Messages.Balances;
using TS.Brokers.States;

namespace TS.Brokers.GrainInterfaces
{
    public interface IBalanceGrain : IGrainWithStringKey
    {
        Task Deposit(BalanceRequestMessage message);

        Task Deposit(decimal value);

        Task<BalanceState> Get();

        Task Update(decimal value);
    }
}