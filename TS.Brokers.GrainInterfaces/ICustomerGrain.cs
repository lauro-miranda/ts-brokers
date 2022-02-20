using LM.Responses;
using Orleans;
using TS.Brokers.Messages;
using TS.Brokers.States;

namespace TS.Brokers.GrainInterfaces
{
    public interface ICustomerGrain : IGrainWithStringKey
    {
        Task<Response> Create(CustomerRequestMessage message);

        Task<CustomerState> Get();

        Task<Response> AddAsset(string symbol, CustomerState.AssetState asset);

        Task UpdatePrice(string symbol, decimal price);

        Task UpdateAsset(Guid code, string symbol, int quantity, decimal salePrice);
    }
}