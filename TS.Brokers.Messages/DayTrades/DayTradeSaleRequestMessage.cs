namespace TS.Brokers.Messages.DayTrades
{
    public class DayTradeSaleRequestMessage
    {
        public string Identification { get; set; } = "";

        public string Symbol { get; set; } = "";

        public int Quantity { get; set; }

        public decimal SalePrice { get; set; }
    }
}