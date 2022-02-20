using LM.Responses;
using Orleans;
using TS.Brokers.Messages.DayTrades;
using TS.Brokers.States;

namespace TS.Brokers.GrainInterfaces
{
    public interface IDayTradeGrain : IGrainWithStringKey
    {
        Task<DayTradeState> Get();

        Task<Response> Purchase(DayTradeRequestMessage message);

        Task<Response> Sell(DayTradeSaleRequestMessage message);
    }
}