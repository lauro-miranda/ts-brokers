using LM.Responses;
using LM.Responses.Extensions;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;
using TS.Brokers.GrainInterfaces;
using TS.Brokers.Messages.DayTrades;
using TS.Brokers.Messages.SwingTrades;

namespace TS.Brokers.Api.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        IClusterClient ClusterClient { get; }

        public OrderController(IClusterClient clusterClient)
        {
            ClusterClient = clusterClient;
        }

        [HttpPost, Route("purchase/dt")]
        public async Task<IActionResult> PurchaseAsync([FromBody] DayTradeRequestMessage message)
        {
            var response = Response<bool>.Create();

            var grain = ClusterClient.GetGrain<IDayTradeGrain>(message.Identification);

            if (response.WithMessages((await grain.Purchase(message)).Messages).HasError)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost, Route("purchase/st")]
        public async Task<IActionResult> PurchaseAsync([FromBody] SwingTradeRequestMessage message)
        {
            var response = Response<bool>.Create();

            var grain = ClusterClient.GetGrain<ISwingTradeGrain>(message.Identification);

            if (response.WithMessages((await grain.Purchase(message)).Messages).HasError)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost, Route("sale/dt")]
        public async Task<IActionResult> PurchaseAsync([FromBody] DayTradeSaleRequestMessage message)
        {
            var response = Response<bool>.Create();

            var grain = ClusterClient.GetGrain<IDayTradeGrain>(message.Identification);

            if (response.WithMessages((await grain.Sell(message)).Messages).HasError)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost, Route("sale/st")]
        public async Task<IActionResult> PurchaseAsync([FromBody] SwingTradeSaleRequestMessage message)
        {
            var response = Response<bool>.Create();

            var grain = ClusterClient.GetGrain<ISwingTradeGrain>(message.Identification);

            if (response.WithMessages((await grain.Sell(message)).Messages).HasError)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet, Route("stock/{identification}/dt")]
        public async Task<IActionResult> GetStockDTAsync(string identification)
        {
            var response = Response<object>.Create();

            var grain = ClusterClient.GetGrain<IDayTradeGrain>(identification);

            var state = await grain.Get();

            return Ok(response.SetValue(state));
        }

        [HttpGet, Route("stock/{identification}/st")]
        public async Task<IActionResult> GetStocksSTAsync(string identification)
        {
            var response = Response<object>.Create();

            var grain = ClusterClient.GetGrain<ISwingTradeGrain>(identification);

            var state = await grain.Get();

            return Ok(response.SetValue(state));
        }
    }
}