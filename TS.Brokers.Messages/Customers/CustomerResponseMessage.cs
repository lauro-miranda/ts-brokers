using System;
using System.Collections.Generic;

namespace TS.Brokers.Messages
{
    public class CustomerResponseMessage
    {
        public string Identification { get; set; } = "";

        public string Name { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public IDictionary<string, List<AssetResponseMessage>> Assets { get; set; } = new Dictionary<string, List<AssetResponseMessage>>();

        public class AssetResponseMessage
        {
            public DateTime CreatedAt { get; set; }

            public DateTime UpdatedAt { get; set; }

            public string ModuleType { get; set; } = "";

            public int Quantity { get; set; }

            public decimal Price { get; set; }

            public decimal PurchasePrice { get; set; }

            public decimal PurchasingPower { get; set; }
        }
    }
}