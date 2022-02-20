namespace TS.Brokers.Messages.Balances
{
    public class BalanceRequestMessage
    {
        public string Identification { get; set; } = "";

        public decimal Value { get; set; }
    }
}