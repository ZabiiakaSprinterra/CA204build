using Newtonsoft.Json;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.API.Entity
{
    public class APIQuantityValue
    {
        [JsonProperty(PropertyName = "Value")]
        public APIUpdates Value { get; set; }
    }

    public class APIUpdates
    {
        [JsonProperty(PropertyName = "UpdateType")]
        public string UpdateType { get; set; }

        [JsonProperty(PropertyName = "Updates")]
        public List<KCAPIQuantity> Updates { get; set; }
    }

    public class KCAPIQuantity
    {
        [JsonProperty(PropertyName = "DistributionCenterID")]
        public int? DistributionCenterID { get; set; }

        [JsonProperty(PropertyName = "Quantity")]
        public int? Quantity { get; set; }
    }
}
