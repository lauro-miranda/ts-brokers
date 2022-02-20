namespace TS.Brokers.Messages.SwingTrades
{
    public class SwingTradeSaleRequestMessage
    {
        public string Identification { get; set; } = "";

        public string Symbol { get; set; } = "";

        public int Quantity { get; set; }

        public decimal SalePrice { get; set; }
    }
}