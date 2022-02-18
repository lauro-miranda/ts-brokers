using LM.Domain.Helpers;
using LM.Responses;
using LM.Responses.Extensions;
using Orleans;
using Orleans.Providers;
using System.Threading.Tasks;
using TS.Brokers.GrainInterfaces;
using TS.Brokers.Messages;
using TS.Brokers.States;

namespace TS.Brokers.Grains
{
    [StorageProvider(ProviderName = "customerStore")]
    public class CustomerGrain : Grain<CustomerState>, ICustomerGrain
    {
        public override async Task OnActivateAsync()
        {
            await ReadStateAsync();
            await base.OnActivateAsync();
        }

        public async Task<Response> Create(CustomerRequestMessage message)
        {
            var response = Response.Create();

            if (!this.GetPrimaryKeyString().Equals(message.Identification))
                response.WithBusinessError(nameof(message.Identification), "A identificação do cliente difere do Id do grão.");

            if (string.IsNullOrEmpty(message.Name))
                response.WithBusinessError(nameof(message.Name), "O nome não foi informado.");

            if (response.HasError) return response;

            State = new CustomerState()
            {
                Identification = message.Identification,
                Name = message.Name,
                CreatedAt = DateTimeHelper.GetCurrentDate()
            };

            await WriteStateAsync();

            return await Task.FromResult(response);
        }

        public async Task<CustomerState> Get()
        {
            return await Task.FromResult(State);
        }
    }
}