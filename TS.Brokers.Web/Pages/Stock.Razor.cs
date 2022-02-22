using LaunchDarkly.EventSource;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace TS.Brokers.Web.Pages
{
    public partial class Stock : ComponentBase, IDisposable
    {
        public ConcurrentDictionary<string, Data.Stock> Stocks { get; set; } = new ConcurrentDictionary<string, Data.Stock>();

        public int CurrentCount { get; set; } = 0;

        System.Timers.Timer Timer { get; set; } = new System.Timers.Timer();

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                Timer.Interval = 1000;
                Timer.Elapsed += Timer_Elapsed;
                Timer.AutoReset = true;
                Timer.Enabled = true;
            }

            base.OnAfterRender(firstRender);
        }

        void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
        }

        protected override void OnInitialized()
        {
            var configuration = Configuration.Builder(new Uri("https://localhost:44360/stock-notifications"));

            var EventSource = new EventSource(configuration.Build());

            EventSource.MessageReceived += EventSource_MessageReceived;

            EventSource.StartAsync().ConfigureAwait(false);
        }

        void EventSource_MessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            var stock = JsonConvert.DeserializeObject<Data.Stock>(e.Message.Data);

            if (stock == null)
                return;

            Stocks[stock.Symbol] = stock;

            InvokeAsync(() => StateHasChanged());

            Console.WriteLine(e.Message.Data);
        }

        public void Dispose()
        {
            Timer?.Dispose();
        }
    }
}