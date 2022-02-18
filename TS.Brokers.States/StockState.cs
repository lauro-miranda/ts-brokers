namespace TS.Brokers.States
{
    public class StockState
    {
        public string Symbol { get; set; } = "";

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}