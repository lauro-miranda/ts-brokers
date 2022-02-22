using Blazored.LocalStorage;
using LM.Responses;
using LM.Responses.Extensions;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http.Formatting;
using TS.Brokers.Web.Data;

namespace TS.Brokers.Web.Pages
{
    public partial class Customer : ComponentBase
    {
        [Inject]
        IHttpClientFactory? HttpClientFactory { get; set; }

        [Inject]
        ILocalStorageService LocalStorageService { get; set; }

        public CustomerData CustomerData { get; } = new CustomerData();

        public decimal Value { get; set; }

        public async Task Create()
        {
            var response = Response.Create();

            var client = HttpClientFactory.CreateClient("ServerAPI");

            if (string.IsNullOrEmpty(CustomerData.Identification))
                response.WithBusinessError("A identificação não foi informada.");

            if (string.IsNullOrEmpty(CustomerData.Name))
                response.WithBusinessError("O nome não foi informado.");

            if (response.HasError)
                return;

            var content = new ObjectContent<object>(new { CustomerData.Identification, CustomerData.Name }, new JsonMediaTypeFormatter());

            var httpResponseMessae = await client.PostAsync("customer", content);

            if (httpResponseMessae.IsSuccessStatusCode)
            {
                await LocalStorageService.SetItemAsync(nameof(User), new User
                {
                    Identification = CustomerData.Identification,
                    Name = CustomerData.Name
                });
                return;
            }

            var httpResponse = JsonConvert.DeserializeObject<Response>(await httpResponseMessae.Content.ReadAsStringAsync());
        }

        public async Task Deposit()
        {
            var response = Response.Create();

            if (Value <= 0)
                response.WithBusinessError("O valor não é válido.");

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
                Identification = user.Identification,
                Value
            }, new JsonMediaTypeFormatter());

            var httpResponseMessae = await client.PostAsync("customer/balance", content);

            if (httpResponseMessae.IsSuccessStatusCode)
                return;

            var httpResponse = JsonConvert.DeserializeObject<Response>(await httpResponseMessae.Content.ReadAsStringAsync());
        }
    }
}