using LM.Domain.Helpers;
using System;
using TS.Brokers.Messages.DayTrades;
using TS.Brokers.Messages.SwingTrades;
using TS.Brokers.States;
using TS.Brokers.States.Shared.Enums;

namespace TS.Brokers.Grains.Mappers
{
    internal static class CustomerMapperExtensions
    {
        public static CustomerState.AssetState CustomerToAssetState(this DayTradeRequestMessage message, decimal price)
        {
            if (message == null)
                return new CustomerState.AssetState();

            return new CustomerState.AssetState
            {
                Code = Guid.NewGuid(),
                ModuleType = ModuleType.DayTrade,
                Price = price,
                PurchasePrice = message.PurchasePrice,
                Quantity = message.Quantity,
                UpdatedAt = DateTimeHelper.GetCurrentDate(),
                CreatedAt = DateTimeHelper.GetCurrentDate()
            };
        }

        public static DayTradeState.AssetState DayTradeToAssetState(this DayTradeRequestMessage message, Guid code, decimal price)
        {
            if (message == null)
                return new DayTradeState.AssetState();

            return new DayTradeState.AssetState
            {
                Code = code,
                Price = price,
                PurchasePrice = message.PurchasePrice,
                Quantity = message.Quantity,
                UpdatedAt = DateTimeHelper.GetCurrentDate(),
                CreatedAt = DateTimeHelper.GetCurrentDate()
            };
        }

        public static CustomerState.AssetState CustomerToAssetState(this SwingTradeRequestMessage message, Guid code, decimal price)
        {
            if (message == null)
                return new CustomerState.AssetState();

            return new CustomerState.AssetState
            {
                Code = code,
                ModuleType = ModuleType.SwingTrade,
                Price = price,
                PurchasePrice = message.PurchasePrice,
                Quantity = message.Quantity,
                UpdatedAt = DateTimeHelper.GetCurrentDate(),
                CreatedAt = DateTimeHelper.GetCurrentDate()
            };
        }

        public static SwingTradeState.AssetState DayTradeToAssetState(this SwingTradeRequestMessage message, Guid code, decimal price)
        {
            if (message == null)
                return new SwingTradeState.AssetState();

            return new SwingTradeState.AssetState
            {
                Code = code,
                Price = price,
                PurchasePrice = message.PurchasePrice,
                Quantity = message.Quantity,
                UpdatedAt = DateTimeHelper.GetCurrentDate(),
                CreatedAt = DateTimeHelper.GetCurrentDate()
            };
        }
    }
}