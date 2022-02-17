using System;

namespace TS.Brokers.States
{
    public class CustomerState
    {
        public string Identification { get; set; } = "";

        public string Name { get; set; } = "";

        public DateTime CreatedAt { get; set; }
    }
}