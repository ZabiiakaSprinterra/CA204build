using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.API.Entity
{
    public class KCAPIFulfillment : KCICAEntity
    {
        [JsonProperty("odata.context")]
        public string Metadata { get; set; }

        [JsonProperty(PropertyName = "ID")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "ProfileID")]
        public int ProfileID { get; set; }

        [JsonProperty(PropertyName = "OrderID")]
        public int? OrderID { get; set; }

        [JsonProperty(PropertyName = "CreatedDateUtc")]
        public DateTimeOffset CreatedDateUtc { get; set; }

        [JsonProperty(PropertyName = "UpdatedDateUtc")]
        public DateTimeOffset UpdatedDateUtc { get; set; }

        [JsonProperty(PropertyName = "Type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "DeliveryStatus")]
        public string DeliveryStatus { get; set; }

        [JsonProperty(PropertyName = "TrackingNumber")]
        public string TrackingNumber { get; set; }

        [JsonProperty(PropertyName = "ShippingCarrier")]
        public string ShippingCarrier { get; set; }

        [JsonProperty(PropertyName = "ShippingClass")]
        public string ShippingClass { get; set; }

        [JsonProperty(PropertyName = "DistributionCenterID")]
        public int? DistributionCenterID{ get; set; }

        [JsonProperty(PropertyName = "ExternalFulfillmentCenterCode")]
        public string ExternalFulfillmentCenterCode { get; set; }

        [JsonProperty(PropertyName = "ShippingCost")]
        public decimal? ShippingCost { get; set; }

        [JsonProperty(PropertyName = "InsuranceCost")]
        public decimal? InsuranceCost { get; set; }

        [JsonProperty(PropertyName = "TaxCost")]
        public decimal? TaxCost { get; set; }

        [JsonProperty(PropertyName = "ShippedDateUtc")]
        public DateTimeOffset? ShippedDateUtc { get; set; }

        [JsonProperty(PropertyName = "SellerFulfillmentID")]
        public string SellerFulfillmentID { get; set; }

        [JsonProperty(PropertyName = "HasShippingLabel")]
        public bool? HasShippingLabel { get; set; }

        [JsonProperty(PropertyName = "LabelFormat")]
        public string LabelFormat { get; set; }

        [JsonProperty(PropertyName = "Items")]
        public List<KCAPIFulfillmentItem> Items { get; set; }
    }

    public class KCAPIFulfillmentItem
    {
        [JsonProperty(PropertyName = "ID")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "ProfileID")]
        public int ProfileID { get; set; }

        [JsonProperty(PropertyName = "FulfillmentID")]
        public int FulfillmentID { get; set; }

        [JsonProperty(PropertyName = "OrderID")]
        public int OrderID { get; set; }

        [JsonProperty(PropertyName = "OrderItemID")]
        public int OrderItemID { get; set; }

        [JsonProperty(PropertyName = "Quantity")]
        public int Quantity { get; set; }

        [JsonProperty(PropertyName = "ProductID")]
        public int ProductID { get; set; }
    }
}
