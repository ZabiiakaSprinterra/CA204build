using Newtonsoft.Json;

namespace KChannelAdvisor.Descriptor.API.Entity
{
    public class KCAPIOrderItem : KCICAEntity
    {
        [JsonProperty("odata.context")]
        public string Metadata { get; set; }

        [JsonProperty(PropertyName = "ID")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "ProfileID")]
        public int ProfileID { get; set; }

        [JsonProperty(PropertyName = "OrderID")]
        public int OrderID { get; set; }

        [JsonProperty(PropertyName = "ProductID")]
        public int ProductID { get; set; }

        [JsonProperty(PropertyName = "SiteOrderItemID")]
        public string SiteOrderItemID { get; set; }

        [JsonProperty(PropertyName = "SellerOrderItemID")]
        public int? SellerOrderItemID { get; set; }

        [JsonProperty(PropertyName = "SiteListingID")]
        public string SiteListingID { get; set; }

        [JsonProperty(PropertyName = "Sku")]
        public string Sku { get; set; }

        [JsonProperty(PropertyName = "Title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "Quantity")]
        public int Quantity { get; set; }

        [JsonProperty(PropertyName = "UnitPrice")]
        public decimal? UnitPrice { get; set; }

        [JsonProperty(PropertyName = "TaxPrice")]
        public decimal TaxPrice { get; set; }

        [JsonProperty(PropertyName = "ShippingPrice")]
        public decimal ShippingPrice { get; set; }

        [JsonProperty(PropertyName = "ShippingTaxPrice")]
        public decimal ShippingTaxPrice { get; set; }

        [JsonProperty(PropertyName = "RecyclingFee")]
        public decimal RecyclingFee { get; set; }

        [JsonProperty(PropertyName = "GiftMessage")]
        public string GiftMessage { get; set; }

        [JsonProperty(PropertyName = "GiftNotes")]
        public string GiftNotes { get; set; }

        [JsonProperty(PropertyName = "GiftPrice")]
        public decimal GiftPrice { get; set; }

        [JsonProperty(PropertyName = "GiftTaxPrice")]
        public decimal GiftTaxPrice { get; set; }

        [JsonProperty(PropertyName = "IsBundle")]
        public bool IsBundle { get; set; }

        [JsonProperty(PropertyName = "ItemURL")]
        public string ItemURL { get; set; }

        [JsonProperty(PropertyName = "HarmonizedCode")]
        public string HarmonizedCode { get; set; }
    }
}
