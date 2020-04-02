using Newtonsoft.Json;
using System;


namespace KChannelAdvisor.Descriptor.API.Entity
{
    public class KCAPIOrder : KCICAEntity
    {
        [JsonProperty("odata.context")]
        public string Metadata { get; set; }

        [JsonProperty(PropertyName = "ID")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "ProfileID")]
        public int ProfileID { get; set; }

        [JsonProperty(PropertyName = "SiteID")]
        public int SiteID { get; set; }

        [JsonProperty(PropertyName = "SiteName")]
        public string SiteName { get; set; }

        [JsonProperty(PropertyName = "SiteAccountID")]
        public int? SiteAccountID { get; set; }

        [JsonProperty(PropertyName = "SiteOrderID")]
        public string SiteOrderID { get; set; }

        [JsonProperty(PropertyName = "SecondarySiteOrderID")]
        public string SecondarySiteOrderID { get; set; }

        [JsonProperty(PropertyName = "SellerOrderID")]
        public string SellerOrderID { get; set; }

        [JsonProperty(PropertyName = "CheckoutSourceID")]
        public byte? CheckoutSourceID { get; set; }

        [JsonProperty(PropertyName = "Currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "CreatedDateUtc")]
        public DateTimeOffset CreatedDateUtc { get; set; }

        [JsonProperty(PropertyName = "ImportDateUtc")]
        public DateTimeOffset ImportDateUtc { get; set; }

        [JsonProperty(PropertyName = "PublicNotes")]
        public string PublicNotes { get; set; }

        [JsonProperty(PropertyName = "PrivateNotes")]
        public string PrivateNotes { get; set; }

        [JsonProperty(PropertyName = "SpecialInstructions")]
        public string SpecialInstructions { get; set; }

        [JsonProperty(PropertyName = "TotalPrice")]
        public decimal TotalPrice { get; set; }

        [JsonProperty(PropertyName = "TotalTaxPrice")]
        public decimal? TotalTaxPrice { get; set; }

        [JsonProperty(PropertyName = "TotalShippingPrice")]
        public decimal? TotalShippingPrice { get; set; }

        [JsonProperty(PropertyName = "TotalShippingTaxPrice")]
        public decimal? TotalShippingTaxPrice { get; set; }

        [JsonProperty(PropertyName = "TotalInsurancePrice")]
        public decimal? TotalInsurancePrice { get; set; }

        [JsonProperty(PropertyName = "TotalGiftOptionPrice")]
        public decimal? TotalGiftOptionPrice { get; set; }

        [JsonProperty(PropertyName = "TotalGiftOptionTaxPrice")]
        public decimal? TotalGiftOptionTaxPrice { get; set; }

        [JsonProperty(PropertyName = "AdditionalCostOrDiscount")]
        public decimal? AdditionalCostOrDiscount { get; set; }

        [JsonProperty(PropertyName = "EstimatedShipDateUtc")]
        public DateTimeOffset? EstimatedShipDateUtc { get; set; }

        [JsonProperty(PropertyName = "DeliverByDateUtc")]
        public DateTimeOffset? DeliverByDateUtc { get; set; }

        [JsonProperty(PropertyName = "ResellerID")]
        public string ResellerID { get; set; }

        [JsonProperty(PropertyName = "FlagID")]
        public string FlagID { get; set; }

        [JsonProperty(PropertyName = "FlagDescription")]
        public string FlagDescription { get; set; }

        [JsonProperty(PropertyName = "OrderTags")]
        public string OrderTags { get; set; }

        [JsonProperty(PropertyName = "DistributionCenterTypeRollup")]
        public string DistributionCenterTypeRollup { get; set; }

        [JsonProperty(PropertyName = "CheckoutStatus")]
        public string CheckoutStatus { get; set; }

        [JsonProperty(PropertyName = "PaymentStatus")]
        public string PaymentStatus { get; set; }

        [JsonProperty(PropertyName = "ShippingStatus")]
        public string ShippingStatus { get; set; }

        [JsonProperty(PropertyName = "CheckoutDateUtc")]
        public DateTimeOffset CheckoutDateUtc { get; set; }

        [JsonProperty(PropertyName = "PaymentDateUtc")]
        public DateTimeOffset PaymentDateUtc { get; set; }

        [JsonProperty(PropertyName = "ShippingDateUtc")]
        public DateTimeOffset ShippingDateUtc { get; set; }

        [JsonProperty(PropertyName = "BuyerUserId")]
        public string BuyerUserId { get; set; }

        [JsonProperty(PropertyName = "BuyerEmailAddress")]
        public string BuyerEmailAddress { get; set; }

        [JsonProperty(PropertyName = "BuyerEmailOptIn")]
        public bool BuyerEmailOptIn { get; set; }

        [JsonProperty(PropertyName = "OrderTaxType")]
        public string OrderTaxType { get; set; }

        [JsonProperty(PropertyName = "ShippingTaxType")]
        public string ShippingTaxType { get; set; }

        [JsonProperty(PropertyName = "GiftOptionsTaxType")]
        public string GiftOptionsTaxType { get; set; }

        [JsonProperty(PropertyName = "PaymentMethod")]
        public string PaymentMethod { get; set; }

        [JsonProperty(PropertyName = "PaymentTransactionID")]
        public string PaymentTransactionID { get; set; }

        [JsonProperty(PropertyName = "PaymentPaypalAccountID")]
        public string PaymentPaypalAccountID { get; set; }

        [JsonProperty(PropertyName = "PaymentCreditCardLast4")]
        public string PaymentCreditCardLast4 { get; set; }

        [JsonProperty(PropertyName = "PaymentMerchantReferenceNumber")]
        public string PaymentMerchantReferenceNumber { get; set; }

        [JsonProperty(PropertyName = "ShippingTitle")]
        public string ShippingTitle { get; set; }

        [JsonProperty(PropertyName = "ShippingFirstName")]
        public string ShippingFirstName { get; set; }

        [JsonProperty(PropertyName = "ShippingLastName")]
        public string ShippingLastName { get; set; }

        [JsonProperty(PropertyName = "ShippingSuffix")]
        public string ShippingSuffix { get; set; }

        [JsonProperty(PropertyName = "ShippingCompanyName")]
        public string ShippingCompanyName { get; set; }

        [JsonProperty(PropertyName = "ShippingCompanyJobTitle")]
        public string ShippingCompanyJobTitle { get; set; }

        [JsonProperty(PropertyName = "ShippingDaytimePhone")]
        public string ShippingDaytimePhone { get; set; }

        [JsonProperty(PropertyName = "ShippingEveningPhone")]
        public string ShippingEveningPhone { get; set; }

        [JsonProperty(PropertyName = "ShippingAddressLine1")]
        public string ShippingAddressLine1 { get; set; }

        [JsonProperty(PropertyName = "ShippingAddressLine2")]
        public string ShippingAddressLine2 { get; set; }

        [JsonProperty(PropertyName = "ShippingCity")]
        public string ShippingCity { get; set; }

        [JsonProperty(PropertyName = "ShippingStateOrProvince")]
        public string ShippingStateOrProvince { get; set; }

        [JsonProperty(PropertyName = "ShippingStateOrProvinceName")]
        public string ShippingStateOrProvinceName { get; set; }

        [JsonProperty(PropertyName = "ShippingPostalCode")]
        public string ShippingPostalCode { get; set; }

        [JsonProperty(PropertyName = "ShippingCountry")]
        public string ShippingCountry { get; set; }

        [JsonProperty(PropertyName = "BillingTitle")]
        public string BillingTitle { get; set; }

        [JsonProperty(PropertyName = "BillingFirstName")]
        public string BillingFirstName { get; set; }

        [JsonProperty(PropertyName = "BillingLastName")]
        public string BillingLastName { get; set; }

        [JsonProperty(PropertyName = "BillingSuffix")]
        public string BillingSuffix { get; set; }

        [JsonProperty(PropertyName = "BillingCompanyName")]
        public string BillingCompanyName { get; set; }

        [JsonProperty(PropertyName = "BillingCompanyJobTitle")]
        public string BillingCompanyJobTitle { get; set; }

        [JsonProperty(PropertyName = "BillingDaytimePhone")]
        public string BillingDaytimePhone { get; set; }

        [JsonProperty(PropertyName = "BillingEveningPhone")]
        public string BillingEveningPhone { get; set; }

        [JsonProperty(PropertyName = "BillingAddressLine1")]
        public string BillingAddressLine1 { get; set; }

        [JsonProperty(PropertyName = "BillingAddressLine2")]
        public string BillingAddressLine2 { get; set; }

        [JsonProperty(PropertyName = "BillingCity")]
        public string BillingCity { get; set; }

        [JsonProperty(PropertyName = "BillingStateOrProvince")]
        public string BillingStateOrProvince { get; set; }

        [JsonProperty(PropertyName = "BillingStateOrProvinceName")]
        public string BillingStateOrProvinceName { get; set; }

        [JsonProperty(PropertyName = "BillingPostalCode")]
        public string BillingPostalCode { get; set; }

        [JsonProperty(PropertyName = "BillingCountry")]
        public string BillingCountry { get; set; }

        [JsonProperty(PropertyName = "PromotionCode")]
        public string PromotionCode { get; set; }

        [JsonProperty(PropertyName = "PromotionAmount")]
        public decimal PromotionAmount { get; set; }
    }
}
