using Orleans;

namespace TS.Brokers.GrainInterfaces
{
    public interface IStockExchangeGrain : IGrainWithStringKey
    {
        Task Start(Guid id, string namespaceName);

        Task StopProducing();
    }
}