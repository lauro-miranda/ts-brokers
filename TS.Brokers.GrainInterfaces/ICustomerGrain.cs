using LM.Responses;
using Orleans;
using System.Threading.Tasks;
using TS.Brokers.Messages;
using TS.Brokers.States;

namespace TS.Brokers.GrainInterfaces
{
    public interface ICustomerGrain : IGrainWithStringKey
    {
        Task<Response> Create(CustomerRequestMessage message);

        Task<CustomerState> Get();
    }
}