using Lib.AspNetCore.ServerSentEvents;
using Microsoft.Extensions.Options;
using TS.Brokers.Api.SSE.Interfaces;

namespace TS.Brokers.Api.SSE
{
    public class StockExchangeServerSentEvents : ServerSentEventsService, IStockExchangeServerSentEvents
    {
        public StockExchangeServerSentEvents(IOptions<ServerSentEventsServiceOptions<ServerSentEventsService>> options) : base(options)
        {
        }
    }
}