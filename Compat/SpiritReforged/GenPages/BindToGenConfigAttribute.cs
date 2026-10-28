using System;

namespace Coralite.Compat.SpiritReforged.GenPages
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class BindToGenConfigAttribute(object min, object max, object step, bool slider = false, bool reverseMinMax = false,
    bool denominator = false, string? priorityConfig = null) : Attribute
    {
        public readonly object Minimum = min;
        public readonly object Maximum = max;
        public readonly object Step = step;
        public readonly bool Slider = slider;
        public readonly bool ReverseMinMax = reverseMinMax;
        public readonly bool Denominator = denominator;
        public readonly string? PriorityConfig = priorityConfig;
    }
}
