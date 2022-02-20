using LM.Domain.Helpers;
using LM.Responses;
using LM.Responses.Extensions;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TS.Brokers.GrainInterfaces;
using TS.Brokers.Messages;
using TS.Brokers.States;

namespace TS.Brokers.Grains
{
    [StorageProvider(ProviderName = "customerStore")]
    public class CustomerGrain : Grain<CustomerState>, ICustomerGrain
    {
        IClusterClient ClusterClient { get; }

        public CustomerGrain(IClusterClient clusterClient)
        {
            ClusterClient = clusterClient;
        }

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

        public Task<CustomerState> Get() => Task.FromResult(State);

        public async Task<Response> AddAsset(string symbol, CustomerState.AssetState asset)
        {
            var response = Response.Create();

            if (string.IsNullOrEmpty(symbol))
                response.WithBusinessError(nameof(symbol), "Símbolo não informado.");

            if (response.HasError)
                return await Task.FromResult(response);

            if (State.Assets.ContainsKey(symbol))
            {
                State.Assets[symbol].Add(asset);
                return await Task.FromResult(response);
            }

            asset.CreatedAt = DateTimeHelper.GetCurrentDate();
            asset.UpdatedAt = DateTimeHelper.GetCurrentDate();
            State.Assets[symbol] = new List<CustomerState.AssetState>(1) { asset };

            return await Task.FromResult(response);
        }

        public async Task UpdateAsset(Guid code, string symbol, int quantity, decimal salePrice)
        {
            var balanceGrain = ClusterClient.GetGrain<IBalanceGrain>(this.GetPrimaryKeyString());

            var balance = await balanceGrain.Get();

            var asset = State.Assets[symbol].FirstOrDefault(a => a.Code.Equals(code));

            if ((asset.Quantity - quantity) == 0)
            {
                State.Assets[symbol].Remove(asset);
            }

            asset.Quantity -= quantity;

            await balanceGrain.Deposit(salePrice * quantity);
        }

        public Task UpdatePrice(string symbol, decimal price)
        {
            State.Assets[symbol].ForEach(asset =>
            {
                asset.Price = price;
                asset.UpdatedAt = DateTimeHelper.GetCurrentDate();
            });

            return Task.CompletedTask;
        }
    }
}