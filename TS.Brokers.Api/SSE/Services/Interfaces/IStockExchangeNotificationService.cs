using System.Threading.Tasks;
using TS.Brokers.Messages.StockExchanges.SSE;

namespace TS.Brokers.Api.SSE.Services.Interfaces
{
    public interface IStockExchangeNotificationService
    {
        Task SendAsync(StockUpdatedMessage message);
    }
}