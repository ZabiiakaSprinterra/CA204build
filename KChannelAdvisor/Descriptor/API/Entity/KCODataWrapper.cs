using Newtonsoft.Json;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.API.Entity
{
    public class KCODataWrapper<T> where T : KCICAEntity
    {
        [JsonProperty("odata.context")]
        public string Metadata { get; set; }

        [JsonProperty("value")]
        public List<T> Value { get; set; }

        [JsonProperty("@odata.nextLink")]
        public string NextLink { get; set; }
    }
}
