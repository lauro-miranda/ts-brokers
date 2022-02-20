using System.ComponentModel;

namespace TS.Brokers.States.Shared.Enums
{
    public enum ModuleType
    {
        [Description("Day Trade")]
        DayTrade = 1,
        [Description("Swing Trade")]
        SwingTrade = 2
    }
}