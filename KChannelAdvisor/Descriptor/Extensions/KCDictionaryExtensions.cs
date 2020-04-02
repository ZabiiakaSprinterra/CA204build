using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.Extensions
{
    public static class KCDictionaryExtensions
    {
        public static TV GetValue<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV))
        {
            return dict.TryGetValue(key, out TV value) ? value : defaultValue;
        }
    }
}
