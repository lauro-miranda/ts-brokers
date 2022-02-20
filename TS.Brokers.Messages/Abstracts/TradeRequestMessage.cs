namespace TS.Brokers.Messages.Abstracts
{
    public class TradeRequestMessage
    {
        public string Identification { get; set; } = "";

        public string Symbol { get; set; } = "";

        public int Quantity { get; set; }

        public decimal PurchasePrice { get; set; }
    }
}