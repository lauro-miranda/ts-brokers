namespace TS.Brokers.Messages.StockExchanges.SSE
{
    public class StockUpdatedMessage
    {
        public string Symbol { get; set; } = "";

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}