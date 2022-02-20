using LM.Responses;
using LM.Responses.Extensions;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Linq;
using System.Threading.Tasks;
using TS.Brokers.GrainInterfaces;
using TS.Brokers.Messages;
using TS.Brokers.Messages.Balances;

namespace TS.Brokers.Api.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        IClusterClient ClusterClient { get; }

        public CustomerController(IClusterClient clusterClient)
        {
            ClusterClient = clusterClient;
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> CreateAsync([FromBody] CustomerRequestMessage message)
        {
            var response = Response<CustomerResponseMessage>.Create();

            var grain = ClusterClient.GetGrain<ICustomerGrain>(message.Identification);

            if (response.WithMessages((await grain.Create(message)).Messages).HasError)
                return BadRequest(response);

            var state = await grain.Get();

            return Ok(response.SetValue(new CustomerResponseMessage
            {
                Name = state.Name,
                CreatedAt = state.CreatedAt,
                Identification = state.Identification
            }));
        }

        [HttpGet, Route("{identification}")]
        public async Task<IActionResult> GetAsync(string identification)
        {
            var response = Response<CustomerResponseMessage>.Create();

            if (string.IsNullOrEmpty(identification))
                return BadRequest(response.WithBusinessError(nameof(identification), "A identificação do cliente não foi informada."));

            var grain = ClusterClient.GetGrain<ICustomerGrain>(identification);

            if (grain == null)
                return NoContent();

            var state = await grain.Get();

            var message = new CustomerResponseMessage
            {
                Name = state.Name,
                CreatedAt = state.CreatedAt,
                Identification = state.Identification,
            };

            foreach (var asset in state.Assets)
            {
                message.Assets.Add(asset.Key, asset.Value.Select(a => new CustomerResponseMessage.AssetResponseMessage
                {
                    ModuleType = a.ModuleType.GetDescription(),
                    Price = a.Price,
                    PurchasePrice = a.PurchasePrice,
                    Quantity = a.Quantity,
                    PurchasingPower = a.PurchasingPower,
                    UpdatedAt = a.UpdatedAt,
                    CreatedAt = a.CreatedAt
                }).ToList());
            }

            return Ok(response.SetValue(message));
        }

        [HttpGet, Route("balance/{identification}")]
        public async Task<IActionResult> DepositAsync(string identification)
        {
            var response = Response<object>.Create(true);

            var balance = await ClusterClient.GetGrain<IBalanceGrain>(identification).Get();

            return Ok(response.SetValue(balance));
        }

        [HttpPost, Route("balance")]
        public async Task<IActionResult> DepositAsync([FromBody] BalanceRequestMessage message)
        {
            var response = Response<bool>.Create(true);

            await ClusterClient.GetGrain<IBalanceGrain>(message.Identification).Deposit(message);

            return Ok(response);
        }
    }
}