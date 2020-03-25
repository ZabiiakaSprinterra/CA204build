using PX.Data;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.Descriptor.API.Mapper
{
    public static class KCResultsetHelper
    {
        public static List<Dictionary<string, object>> GetRowsAsDictionary(PXCache cache, IEnumerable<PXResult> result)
        {
            return result.Select(cache.ToDictionary).ToList();
        }
    }
}
