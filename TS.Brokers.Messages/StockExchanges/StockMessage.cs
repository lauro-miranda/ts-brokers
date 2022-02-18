namespace TS.Brokers.Messages.StockExchanges
{
    public class StockMessage
    {
        public string Symbol { get; set; } = "";

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}