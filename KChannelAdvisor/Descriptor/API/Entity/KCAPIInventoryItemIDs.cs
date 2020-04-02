using Newtonsoft.Json;

namespace KChannelAdvisor.Descriptor.API.Entity
{
    public class KCAPIInventoryItemIDs : KCICAEntity
    {
        [JsonProperty("ID")]
        public int? ID { get; set; }

        [JsonProperty("ParentProductID")]
        public string ParentProductID { get; set; }
    }
}
