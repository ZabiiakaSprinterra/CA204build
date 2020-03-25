using System;

namespace KChannelAdvisor.Descriptor.Extensions
{
    public static class KCDateTimeExtensions
    {
        public static bool BiggerThan(this DateTime a, DateTime b)
        {
            TimeSpan span = new TimeSpan(a.Ticks - b.Ticks);
            return span.Days > 0 || span.Hours > 0 || span.Minutes > 0 || span.Seconds > 1.5;
        }

        public static bool BiggerThan(this DateTime a, long? bTicks)
        {
            TimeSpan span = new TimeSpan(a.Ticks - bTicks.GetValueOrDefault());
            return span.Days > 0 || span.Hours > 0 || span.Minutes > 0 || span.Seconds > 1.5;
        }

        public static bool LessThan(this DateTime a, DateTime b) => BiggerThan(b, a);
    }
}
