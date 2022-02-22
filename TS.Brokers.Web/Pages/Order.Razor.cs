using Blazored.LocalStorage;
using LM.Responses;
using LM.Responses.Extensions;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http.Formatting;

namespace TS.Brokers.Web.Pages
{
    public partial class Order
    {
        [Inject]
        IHttpClientFactory? HttpClientFactory { get; set; }

        [Inject]
        ILocalStorageService LocalStorageService { get; set; }

        public string Symbol { get; set; } = "";

        public int Quantity { get; set; }

        public decimal PurchasePrice { get; set; }

        public async Task Purchase()
        {
            var response = Response.Create();

            if (string.IsNullOrEmpty(Symbol))
                response.WithBusinessError("A identificação não foi informada.");

            if (Quantity <= 0)
                response.WithBusinessError("A quantida não é válida.");

            if (PurchasePrice <= 0)
                response.WithBusinessError("O valor de compra não é valido.");

            if (response.HasError)
                return;

            var user = await LocalStorageService.GetItemAsync<User>(nameof(User));

            if (user == null)
            {
                response.WithBusinessError("Por favor, realize o seu cadastro antes em /customer");
                return;
            }

            var client = HttpClientFactory.CreateClient("ServerAPI");

            var content = new ObjectContent<object>(new
            {
                Identification = "123",
                Symbol,
                Quantity,
                PurchasePrice
            }, new JsonMediaTypeFormatter());

            var httpResponseMessae = await client.PostAsync("order/purchase/st", content);

            if (httpResponseMessae.IsSuccessStatusCode)
                return;

            var httpResponse = JsonConvert.DeserializeObject<Response>(await httpResponseMessae.Content.ReadAsStringAsync());
        }
    }
}