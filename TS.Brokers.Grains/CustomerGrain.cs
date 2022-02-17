using LM.Domain.Helpers;
using LM.Responses;
using LM.Responses.Extensions;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using System.Threading.Tasks;
using TS.Brokers.GrainInterfaces;
using TS.Brokers.Messages;
using TS.Brokers.States;

namespace TS.Brokers.Grains
{
    [StorageProvider(ProviderName = "customerStore")]
    public class CustomerGrain : Grain, ICustomerGrain
    {
        IPersistentState<CustomerState> Customer { get; }

        public CustomerGrain([PersistentState("customer", "customerStore")] IPersistentState<CustomerState> state)
        {
            Customer = state;
        }

        public async Task<Response> Create(CustomerRequestMessage message)
        {
            var response = Response.Create();

            if (!this.GetPrimaryKeyString().Equals(message.Identification))
                response.WithBusinessError(nameof(message.Identification), "A identificação do cliente difere do Id do grão.");

            if (string.IsNullOrEmpty(message.Name))
                response.WithBusinessError(nameof(message.Name), "O nome não foi informado.");

            if (response.HasError) return response;

            Customer.State = new CustomerState()
            {
                Identification = message.Identification,
                Name = message.Name,
                CreatedAt = DateTimeHelper.GetCurrentDate()
            };

            await Customer.WriteStateAsync();

            return await Task.FromResult(response);
        }

        public async Task<CustomerState> Get()
        {
            return await Task.FromResult(Customer.State);
        }
    }
}