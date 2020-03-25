using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.API.Entity
{
    public class ODataShipment
    {
        [JsonProperty(PropertyName = "Value")]
        public KCAPIShipment Value { get; set; }
    }

    public class KCAPIShipment
    {
        [JsonProperty(PropertyName = "ShippedDateUtc")]
        public DateTimeOffset? ShippedDateUtc { get; set; }

        [JsonProperty(PropertyName = "TrackingNumber")]
        public string TrackingNumber { get; set; }

        [JsonProperty(PropertyName = "ShippingCarrier")]
        public string ShippingCarrier { get; set; }

        [JsonProperty(PropertyName = "ShippingClass")]
        public string ShippingClass { get; set; }

        [JsonProperty(PropertyName = "DeliveryStatus")]
        public string DeliveryStatus { get; set; }

        [JsonProperty(PropertyName = "Items")]
        public List<KCAPIShipmentItem> Items { get; set; }
    }

    public class KCAPIShipmentItem
    {
        [JsonProperty(PropertyName = "OrderItemID")]
        public string OrderItemID { get; set; }

        [JsonProperty(PropertyName = "ProductID")]
        public string ProductID { get; set; }

        [JsonProperty(PropertyName = "Quantity")]
        public int Quantity { get; set; }
    }
}
