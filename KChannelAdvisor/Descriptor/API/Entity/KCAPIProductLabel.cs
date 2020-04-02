using Newtonsoft.Json;

namespace KChannelAdvisor.Descriptor.API.Entity
{
    public class KCAPIProductLabel : KCICAEntity
    {
        [JsonProperty("ProductID")]
        public int ProductID { get; set; }
        [JsonProperty("ProfileID")]
        public int ProfileID { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
    }
}
