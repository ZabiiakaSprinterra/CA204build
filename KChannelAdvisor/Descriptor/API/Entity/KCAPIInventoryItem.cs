using KChannelAdvisor.Descriptor.BulkUploader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KChannelAdvisor.Descriptor.API.Entity
{
    public class KCAPIInventoryItem : KCICAEntity
    {
        [JsonProperty(PropertyName = "ID")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "ProfileID")]
        public int? ProfileID { get; set; }

        [JsonProperty(PropertyName = "CreateDateUtc")]
        public DateTimeOffset CreateDateUtc { get; set; }

        [JsonProperty(PropertyName = "UpdateDateUtc")]
        public DateTimeOffset? UpdateDateUtc { get; set; }

        [JsonProperty(PropertyName = "QuantityUpdateDateUtc")]
        public DateTimeOffset? QuantityUpdateDateUtc { get; set; }

        [JsonProperty(PropertyName = "IsAvailableInStore")]
        public bool? IsAvailableInStore { get; set; }

        [JsonProperty(PropertyName = "IsBlocked")]
        public bool? IsBlocked { get; set; }

        [DisplayName(KCHeaders.BlockExternalQuantity)]
        [JsonProperty(PropertyName = "IsExternalQuantityBlocked")]
        public bool? IsExternalQuantityBlocked { get; set; }

        [JsonProperty(PropertyName = "BlockComment")]
        public string BlockComment { get; set; }

        [JsonProperty(PropertyName = "BlockedDateUtc")]
        public DateTimeOffset? BlockedDateUtc { get; set; }

        [JsonProperty(PropertyName = "ReceivedDateUtc")]
        public DateTimeOffset? ReceivedDateUtc { get; set; }

        [JsonProperty(PropertyName = "LastSaleDateUtc")]
        public DateTimeOffset? LastSaleDateUtc { get; set; }

        [DisplayName(KCHeaders.ASIN)]
        [JsonProperty(PropertyName = "ASIN")]
        public string ASIN { get; set; }

        [DisplayName(KCHeaders.Brand)]
        [JsonProperty(PropertyName = "Brand")]
        public string Brand { get; set; }

        [DisplayName(KCHeaders.Condition)]
        [JsonProperty(PropertyName = "Condition")]
        public string Condition { get; set; }

        [DisplayName(KCHeaders.Description)]
        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

        [DisplayName(KCHeaders.EAN)]
        [JsonProperty(PropertyName = "EAN")]
        public string EAN { get; set; }

        [DisplayName(KCHeaders.FlagDescription)]
        [JsonProperty(PropertyName = "FlagDescription")]
        public string FlagDescription { get; set; }

        [DisplayName(KCHeaders.Flag)]
        [JsonProperty(PropertyName = "Flag")]
        public string Flag { get; set; }

        [DisplayName(KCHeaders.HarmonizedCode)]
        [JsonProperty(PropertyName = "HarmonizedCode")]
        public string HarmonizedCode { get; set; }

        [DisplayName(KCHeaders.ISBN)]
        [JsonProperty(PropertyName = "ISBN")]
        public string ISBN { get; set; }

        [DisplayName(KCHeaders.Manufacturer)]
        [JsonProperty(PropertyName = "Manufacturer")]
        public string Manufacturer { get; set; }

        [DisplayName(KCHeaders.MPN)]
        [JsonProperty(PropertyName = "MPN")]
        public string MPN { get; set; }

        [DisplayName(KCHeaders.ShortDescription)]
        [JsonProperty(PropertyName = "ShortDescription")]
        public string ShortDescription { get; set; }

        [DisplayName(KCHeaders.InventoryNumber)]
        [JsonProperty(PropertyName = "Sku")]
        public string Sku { get; set; }

        [JsonProperty(PropertyName = "Subtitle")]
        public string Subtitle { get; set; }

        [DisplayName(KCHeaders.TaxProductCode)]
        [JsonProperty(PropertyName = "TaxProductCode")]
        public string TaxProductCode { get; set; }

        [DisplayName(KCHeaders.AuctionTitle)]
        [JsonProperty(PropertyName = "Title")]
        public string Title { get; set; }

        [DisplayName(KCHeaders.UPC)]
        [JsonProperty(PropertyName = "UPC")]
        public string UPC { get; set; }

        [DisplayName(KCHeaders.WarehouseLocation)]
        [JsonProperty(PropertyName = "WarehouseLocation")]
        public string WarehouseLocation { get; set; }

        [DisplayName(KCHeaders.Warranty)]
        [JsonProperty(PropertyName = "Warranty")]
        public string Warranty { get; set; }

        [DisplayName(KCHeaders.MultipackQuantity)]
        [JsonProperty(PropertyName = "MultipackQuantity")]
        public string MultipackQuantity { get; set; }

        [DisplayName(KCHeaders.Height)]
        [JsonProperty(PropertyName = "Height")]
        public decimal? Height { get; set; }

        [DisplayName(KCHeaders.Length)]
        [JsonProperty(PropertyName = "Length")]
        public decimal? Length { get; set; }

        [DisplayName(KCHeaders.Width)]
        [JsonProperty(PropertyName = "Width")]
        public decimal? Width { get; set; }

        [DisplayName(KCHeaders.Weight)]
        [JsonProperty(PropertyName = "Weight")]
        public decimal? Weight { get; set; }

        [DisplayName(KCHeaders.SellerCost)]
        [JsonProperty(PropertyName = "Cost")]
        public decimal? Cost { get; set; }

        [DisplayName(KCHeaders.ProductMargin)]
        [JsonProperty(PropertyName = "Margin")]
        public decimal? Margin { get; set; }

        [DisplayName(KCHeaders.RetailPrice)]
        [JsonProperty(PropertyName = "RetailPrice")]
        public decimal? RetailPrice { get; set; }

        [DisplayName(KCHeaders.StartingBid)]
        [JsonProperty(PropertyName = "StartingPrice")]
        public decimal? StartingPrice { get; set; }

        [DisplayName(KCHeaders.Reserve)]
        [JsonProperty(PropertyName = "ReservePrice")]
        public decimal? ReservePrice { get; set; }

        [DisplayName(KCHeaders.BuyItNowPrice)]
        [JsonProperty(PropertyName = "BuyItNowPrice")]
        public decimal? BuyItNowPrice { get; set; }

        [DisplayName(KCHeaders.StorePrice)]
        [JsonProperty(PropertyName = "StorePrice")]
        public decimal? StorePrice { get; set; }

        [DisplayName(KCHeaders.SecondChanceOfferPrice)]
        [JsonProperty(PropertyName = "SecondChancePrice")]
        public decimal? SecondChancePrice { get; set; }

        [DisplayName(KCHeaders.MinimumPrice)]
        [JsonProperty(PropertyName = "MinPrice")]
        public decimal? MinPrice { get; set; }

        [DisplayName(KCHeaders.MaximumPrice)]
        [JsonProperty(PropertyName = "MaxPrice")]
        public decimal? MaxPrice { get; set; }

        [JsonProperty(PropertyName = "SupplierName")]
        public string SupplierName { get; set; }

        [DisplayName(KCHeaders.SupplierCode)]
        [JsonProperty(PropertyName = "SupplierCode")]
        public string SupplierCode { get; set; }

        [DisplayName(KCHeaders.SupplierPO)]
        [JsonProperty(PropertyName = "SupplierPO")]
        public string SupplierPO { get; set; }

        [DisplayName(KCHeaders.Classification)]
        [JsonProperty(PropertyName = "Classification")]
        public string Classification { get; set; }

        [JsonProperty(PropertyName = "IsDisplayInStore")]
        public bool? IsDisplayInStore { get; set; }

        [JsonProperty(PropertyName = "StoreTitle")]
        public string StoreTitle { get; set; }

        [JsonProperty(PropertyName = "StoreDescription")]
        public string StoreDescription { get; set; }

        [JsonProperty(PropertyName = "BundleType")]
        public string BundleType { get; set; }

        [JsonProperty(PropertyName = "TotalAvailableQuantity")]
        public int TotalAvailableQuantity { get; set; }

        [JsonProperty(PropertyName = "OpenAllocatedQuantity")]
        public int OpenAllocatedQuantity { get; set; }

        [JsonProperty(PropertyName = "OpenAllocatedQuantityPooled")]
        public int OpenAllocatedQuantityPooled { get; set; }

        [JsonProperty(PropertyName = "PendingCheckoutQuantity")]
        public int PendingCheckoutQuantity { get; set; }

        [JsonProperty(PropertyName = "PendingCheckoutQuantityPooled")]
        public int PendingCheckoutQuantityPooled { get; set; }

        [JsonProperty(PropertyName = "PendingPaymentQuantity")]
        public int PendingPaymentQuantity { get; set; }

        [JsonProperty(PropertyName = "PendingPaymentQuantityPooled")]
        public int PendingPaymentQuantityPooled { get; set; }

        [JsonProperty(PropertyName = "PendingShipmentQuantity")]
        public int PendingShipmentQuantity { get; set; }

        [JsonProperty(PropertyName = "PendingShipmentQuantityPooled")]
        public int PendingShipmentQuantityPooled { get; set; }

        [JsonProperty(PropertyName = "TotalQuantity")]
        public int TotalQuantity { get; set; }

        [JsonProperty(PropertyName = "TotalQuantityPooled")]
        public int TotalQuantityPooled { get; set; }

        [JsonProperty(PropertyName = "IsParent")]
        public bool? IsParent { get; set; }

        [JsonProperty(PropertyName = "IsInRelationship")]
        public bool? IsInRelationship { get; set; }

        [JsonProperty(PropertyName = "ParentProductID")]
        public int? ParentProductID { get; set; }

        [DisplayName(KCHeaders.RelationshipName)]
        [JsonProperty(PropertyName = "RelationshipName")]
        public string RelationshipName { get; set; }

        [JsonProperty(PropertyName = "USP")]
        public string USP { get; set; }

        [JsonProperty(PropertyName = "CategoryPath")]
        public string CategoryPath { get; set; }

        [JsonProperty(PropertyName = "CategoryCode")]
        public int? CategoryCode { get; set; }

        [JsonProperty(PropertyName = "BundleComponents")]
        public List<KCAPIBundleComponent> BundleComponents { get; set; }

        [JsonProperty(PropertyName = "Ad Template Name")]
        public int? AdTemplateName { get; set; }

        [DisplayName(KCHeaders.QuantityUpdateType)]
        [JsonIgnore]
        public string QuantityUpdateType { get; set; }
        
        [DisplayName(KCHeaders.DCQuantity)]
        [JsonIgnore]
        public string DCQuantity { get; set; }

        [DisplayName(KCHeaders.DCQuantityUpdateType)]
        [JsonIgnore]
        public string DCQuantityUpdateType { get; set; }

        [DisplayName(KCHeaders.Labels)]
        [JsonIgnore]
        public string Labels { get; set; }

        [DisplayName(KCHeaders.PictureURLs)]
        [JsonIgnore]
        public string PictureUrls { get; set; }

        [DisplayName(KCHeaders.VariationParentSKU)]
        [JsonIgnore]
        public string VariationParentSku { get; set; }
        
        [DisplayName(KCHeaders.BundleComponents)]
        [JsonIgnore]
        public string FtpBundleComponents { get; set; }
    }
}
