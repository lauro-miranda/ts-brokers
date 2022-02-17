using System;

namespace TS.Brokers.Messages
{
    public class CustomerResponseMessage
    {
        public string Identification { get; set; } = "";

        public string Name { get; set; } = "";

        public DateTime CreatedAt { get; set; }
    }
}