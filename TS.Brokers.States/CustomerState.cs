using System;
using System.Collections.Generic;
using TS.Brokers.States.Shared.Enums;

namespace TS.Brokers.States
{
    public class CustomerState
    {
        public string Identification { get; set; } = "";

        public string Name { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public IDictionary<string, List<AssetState>> Assets { get; set; } = new Dictionary<string, List<AssetState>>();

        public class AssetState
        {
            public Guid Code { get; set; }

            public DateTime CreatedAt { get; set; }

            public DateTime UpdatedAt { get; set; }

            public ModuleType ModuleType { get; set; }

            public int Quantity { get; set; }

            public decimal Price { get; set; }

            public decimal PurchasePrice { get; set; }

            public decimal PurchasingPower => Price * 0.8m;
        }
    }
}