using Newtonsoft.Json;

namespace KChannelAdvisor.Descriptor.API.Entity
{
    public class KCAPIAttribute : KCICAEntity
    {
        [JsonProperty("ProductID")]
        public int? ProductID { get; set; }

        [JsonProperty("ProfileID")]
        public int? ProfileID { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Value")]
        public string Value { get; set; }
    }
}
