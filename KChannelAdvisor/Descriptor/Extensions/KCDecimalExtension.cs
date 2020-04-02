using System;

namespace KChannelAdvisor.Descriptor.Extensions
{
    public static class KCDecimalExtension
    {
        public static decimal? Round(this decimal? num)
        {
            return (decimal)Math.Round(Convert.ToDouble(num), 2);
        }
    }
}
