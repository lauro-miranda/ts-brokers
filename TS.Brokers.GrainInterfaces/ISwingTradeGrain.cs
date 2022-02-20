using LM.Responses;
using Orleans;
using TS.Brokers.Messages.SwingTrades;
using TS.Brokers.States;

namespace TS.Brokers.GrainInterfaces
{
    public interface ISwingTradeGrain : IGrainWithStringKey
    {
        Task<SwingTradeState> Get();

        Task<Response> Purchase(SwingTradeRequestMessage message);

        Task<Response> Sell(SwingTradeSaleRequestMessage message);
    }
}