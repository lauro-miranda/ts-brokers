using Orleans;

namespace TS.Brokers.GrainInterfaces
{
    public interface IStockExchangeGrain : IGrainWithStringKey
    {
        Task SubscribeAsync(Guid id, string namespaceName);

        Task StopProducing();

        Task<decimal> GetPrice();
    }
}