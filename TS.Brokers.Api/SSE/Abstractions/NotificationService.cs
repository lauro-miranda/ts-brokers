using Lib.AspNetCore.ServerSentEvents;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TS.Brokers.Api.SSE.Interfaces;
using TS.Brokers.Messages.StockExchanges.SSE;

namespace TS.Brokers.Api.SSE.Abstractions
{
    public class NotificationService
    {
        IStockExchangeServerSentEvents StockExchangeServerSentEvents { get; }

        public NotificationService(IStockExchangeServerSentEvents stockExchangeServerSentEvents)
        {
            StockExchangeServerSentEvents = stockExchangeServerSentEvents;
        }

        protected async Task SendSseEventAsync(StockUpdatedMessage message)
        {
            await StockExchangeServerSentEvents.SendEventAsync(new ServerSentEvent
            {
                Type = "Update",
                Data = new List<string> { JsonConvert.SerializeObject(message) }
            });
        }
    }
}