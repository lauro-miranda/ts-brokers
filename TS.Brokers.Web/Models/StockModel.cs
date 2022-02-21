namespace TS.Brokers.Web.Models
{
    public class StockModel
    {
        public string Symbol { get; set; } = "";

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}