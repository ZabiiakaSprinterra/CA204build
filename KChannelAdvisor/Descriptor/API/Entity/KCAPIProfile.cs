using Newtonsoft.Json;

namespace KChannelAdvisor.Descriptor.API.Entity
{
    public class KCAPIProfile : KCICAEntity
    {
        [JsonProperty("ID")]
        public int ID { get; set; }
        [JsonProperty("AccountName")]
        public string AccountName { get; set; }
        [JsonProperty("CompanyName")]
        public string CompanyName { get; set; }
        [JsonProperty("CurrencyCode")]
        public string CurrencyCode { get; set; }
        [JsonProperty("TimeZoneRegion")]
        public string TimeZoneRegion { get; set; }
        [JsonProperty("TimeZoneDescription")]
        public string TimeZoneDescription { get; set; }
        [JsonProperty("DefaultDistributionCenterID")]
        public int DefaultDistributionCenterID { get; set; }
    }
}
