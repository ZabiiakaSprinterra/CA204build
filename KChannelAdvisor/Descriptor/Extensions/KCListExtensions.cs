using System;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.Extensions
{
    public static class KCListExtensions
    {
        public static IEnumerable<List<T>> SplitList<T>(this List<T> initialList, int nSize)
        {
            for (int i = 0; i < initialList.Count; i += nSize)
            {
                yield return initialList.GetRange(i, Math.Min(nSize, initialList.Count - i));
            }
        }
    }
}
