using System.Threading.Tasks;
using TS.Brokers.Api.SSE.Abstractions;
using TS.Brokers.Api.SSE.Interfaces;
using TS.Brokers.Api.SSE.Services.Interfaces;
using TS.Brokers.Messages.StockExchanges.SSE;

namespace TS.Brokers.Api.SSE.Services
{
    public class StockExchangeService : NotificationService, IStockExchangeNotificationService
    {
        public StockExchangeService(IStockExchangeServerSentEvents stockExchangeServerSentEvents)
            : base(stockExchangeServerSentEvents)
        {
        }

        public async Task SendAsync(StockUpdatedMessage message) => await SendSseEventAsync(message);
    }
}