using Newtonsoft.Json;

namespace KChannelAdvisor.Descriptor.API.Entity
{
    public class KCAPIIdSkuJuxtaposion : KCICAEntity
    {
        [JsonProperty(PropertyName = "ID")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "Sku")]
        public string Sku { get; set; }

        [JsonProperty(PropertyName = "ParentProductID")]
        public int? ParentProductID { get; set; }
    }
}
