using LM.Domain.Helpers;
using LM.Responses;
using LM.Responses.Extensions;
using Orleans;
using Orleans.Providers;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TS.Brokers.GrainInterfaces;
using TS.Brokers.Grains.Mappers;
using TS.Brokers.Messages.SwingTrades;
using TS.Brokers.States;

namespace TS.Brokers.Grains
{
    [StorageProvider(ProviderName = "swingTradeStore")]
    public class SwingTradeGrain : Grain<SwingTradeState>, ISwingTradeGrain
    {
        IClusterClient ClusterClient { get; }

        public SwingTradeGrain(IClusterClient clusterClient)
        {
            ClusterClient = clusterClient;
        }

        public async Task<Response> Purchase(SwingTradeRequestMessage message)
        {
            var response = Response.Create();

            var balanceGrain = ClusterClient.GetGrain<IBalanceGrain>(message.Identification);

            var balance = await balanceGrain.Get();

            var necessaryValue = message.Quantity * message.PurchasePrice;
            if (balance.Value < necessaryValue)
                return response.WithBusinessError($"Você não possui saldo suficiente para essa compra. " +
                    $"Saldo necesário: {necessaryValue} " +
                    $"- Saldo atual: {balance.Value} " +
                    $"- Faltante: {necessaryValue - balance.Value}");

            var price = await ClusterClient.GetGrain<IStockExchangeGrain>(message.Symbol).GetPrice();

            var asset = message.CustomerToAssetState(Guid.NewGuid(), price);

            await ClusterClient.GetGrain<ICustomerGrain>(message.Identification).AddAsset(message.Symbol, asset);

            if (string.IsNullOrEmpty(State.Identification))
            {
                State = new SwingTradeState { Identification = message.Identification };
            }

            if (State.Assets.ContainsKey(message.Symbol))
            {
                State.Assets[message.Symbol].Add(message.DayTradeToAssetState(asset.Code, price));
            }
            else
            {
                State.Assets[message.Symbol] = new() { message.DayTradeToAssetState(asset.Code, price) };
            }

            await balanceGrain.Update(balance.Value - necessaryValue);

            await SubscribeAsync(message.Symbol);

            return await Task.FromResult(response);
        }

        public async Task<Response> Sell(SwingTradeSaleRequestMessage message)
        {
            var response = Response.Create();

            var assetsQuantity = State.Assets[message.Symbol].Sum(x => x.Quantity);
            if (assetsQuantity < message.Quantity)
                return response.WithBusinessError($"Você possui menos que a quantidade informada disponível para venda. " +
                    $"Quantidade de {message.Symbol} atual: {assetsQuantity}");

            var customer = ClusterClient.GetGrain<ICustomerGrain>(message.Identification);
            var totalQuantity = 0;

            State.Assets[message.Symbol].OrderBy(a => a.PurchasePrice).Where(a => a.Quantity > 0).ToList().ForEach(async asset =>
            {
                if (totalQuantity == message.Quantity)
                    return;

                var quantityToRemove = (message.Quantity - totalQuantity);
                if (asset.Quantity >= quantityToRemove)
                {
                    totalQuantity += asset.RemoveQuantity(quantityToRemove);

                    await customer.UpdateAsset(asset.Code, message.Symbol, quantityToRemove, message.SalePrice);
                }
                else
                {
                    var removedQuantity = asset.RemoveQuantity(asset.Quantity);
                    totalQuantity += removedQuantity;

                    await customer.UpdateAsset(asset.Code, message.Symbol, removedQuantity, message.SalePrice);
                }
            });

            return await Task.FromResult(response);
        }

        public Task<SwingTradeState> Get() => Task.FromResult(State);

        async Task SubscribeAsync(string symbol)
        {
            var grain = ClusterClient.GetGrain<IStockExchangeGrain>(symbol);

            var id = Guid.NewGuid();
            await grain.SubscribeAsync(id, "stock-namespace");

            var stream = ClusterClient.GetStreamProvider("stock-stream-provider")
                .GetStream<StockState>(id, "stock-namespace");

            await stream.SubscribeAsync(OnNextAsync);
        }

        async Task OnNextAsync(StockState state, StreamSequenceToken token = null)
        {
            State.Assets[state.Symbol].ForEach(a =>
            {
                a.Price = state.Price;
                a.UpdatedAt = DateTimeHelper.GetCurrentDate();
            });

            await ClusterClient.GetGrain<ICustomerGrain>(this.GetPrimaryKeyString()).UpdatePrice(state.Symbol, state.Price);
        }
    }
}