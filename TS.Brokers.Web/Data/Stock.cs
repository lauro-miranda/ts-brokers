namespace TS.Brokers.Web.Data
{
    public class Stock
    {
        public string Symbol { get; set; } = "";

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}