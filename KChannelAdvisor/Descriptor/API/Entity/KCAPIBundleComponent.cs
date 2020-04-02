using Newtonsoft.Json;

namespace KChannelAdvisor.Descriptor.API.Entity
{
    public class KCAPIBundleComponent : KCICAEntity
    {
        [JsonProperty("ProductID")]
        public int ProductID { get; set; }

        [JsonProperty("ComponentID")]
        public int? ComponentID { get; set; }

        [JsonProperty("ProfileID")]
        public int? ProfileID { get; set; }

        [JsonProperty("ComponentSku")]
        public string ComponentSku { get; set; }

        [JsonProperty("Quantity")]
        public int? Quantity { get; set; }
    }
}
