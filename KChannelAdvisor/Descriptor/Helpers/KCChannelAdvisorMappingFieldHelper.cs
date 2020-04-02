using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using KChannelAdvisor.DAC;
using PX.Data;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.Helpers
{
    public class KCChannelAdvisorMappingFieldHelper
    {
        private List<KCChannelAdvisorMappingField> _orderFields;
        private List<KCChannelAdvisorMappingField> _productFields;

        public IEnumerable<KCChannelAdvisorMappingField> GetFields(string actionType)
        {
            switch (actionType)
            {
                case KCMappingEntitiesConstants.Order:
                    return GetOrdersSchema();
                case KCMappingEntitiesConstants.Product:
                    return GetProductsSchema();
            }

            throw new PXArgumentException();
        }

        private IEnumerable<KCChannelAdvisorMappingField> GetOrdersSchema()
        {
            _orderFields = new List<KCChannelAdvisorMappingField>
            {
                new KCChannelAdvisorMappingField{ FieldName = "ID"                            , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ProfileID"                     , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "SiteID"                        , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "SiteName"                      , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "SiteAccountID"                 , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "SiteOrderID"                   , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "SecondarySiteOrderID"          , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "SellerOrderID"                 , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "CheckoutSourceID"              , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "Currency"                      , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ImportDateUtc"                 , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "CreatedDateUtc"                , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "PublicNotes"                   , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "PrivateNotes"                  , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "SpecialInstructions"           , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "TotalPrice"                    , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "TotalTaxPrice"                 , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "TotalShippingPrice"            , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "TotalShippingTaxPrice"         , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "TotalInsurancePrice"           , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "TotalGiftOptionPrice"          , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "TotalGiftOptionTaxPrice"       , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "AdditionalCostOrDiscount"      , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "EstimatedShipDateUtc"          , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "DeliverByDateUtc"              , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ResellerID"                    , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "FlagID"                        , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "FlagDescription"               , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "CheckoutStatus"                , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "PaymentStatus"                 , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingStatus"                , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "CheckoutDateUtc"               , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "PaymentDateUtc"                , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingDateUtc"               , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BuyerUserId"                   , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BuyerEmailAddress"             , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BuyerEmailOptIn"               , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "OrderTaxType"                  , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingTaxType"               , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "GiftOptionsTaxType"            , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "PaymentMethod"                 , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "PaymentTransactionID"          , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "PaymentPaypalAccountID"        , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "PaymentCreditCardLast4"        , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "PaymentMerchantReferenceNumber", EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingTitle"                 , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingFirstName"             , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingLastName"              , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingSuffix"                , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingCompanyName"           , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingCompanyJobTitle"       , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingDaytimePhone"          , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingEveningPhone"          , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingAddressLine1"          , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingAddressLine2"          , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingCity"                  , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingStateOrProvince"       , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingStateOrProvinceName"   , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingPostalCode"            , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "ShippingCountry"               , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BillingTitle"                  , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BillingFirstName"              , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BillingLastName"               , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BillingSuffix"                 , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BillingCompanyName"            , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BillingCompanyJobTitle"        , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BillingDaytimePhone"           , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BillingEveningPhone"           , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BillingAddressLine1"           , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BillingAddressLine2"           , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BillingCity"                   , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BillingStateOrProvince"        , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BillingStateOrProvinceName"    , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BillingPostalCode"             , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "BillingCountry"                , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "PromotionCode"                 , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "PromotionAmount"               , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "OrderTags"                     , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "DistributionCenterTypeRollup"  , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "Items"                         , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "Fulfillments"                  , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "Adjustments"                   , EntityType = KCMappingEntitiesConstants.Order },
                new KCChannelAdvisorMappingField{ FieldName = "CustomFields"                  , EntityType = KCMappingEntitiesConstants.Order },
            };

            return _orderFields;
        }

        private IEnumerable<KCChannelAdvisorMappingField> GetProductsSchema()
        {
            _productFields = new List<KCChannelAdvisorMappingField>
            {
                new KCChannelAdvisorMappingField{ FieldName = "ID"                           , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "ProfileID"                    , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "CreateDateUtc"                , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "IsInRelationship"             , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "IsParent"                     , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "RelationshipName"             , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "ParentProductID"              , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "IsAvailableInStore"           , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "IsBlocked"                    , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "IsExternalQuantityBlocked"    , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "BlockComment"                 , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "BlockedDateUtc"               , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "ReceivedDateUtc"              , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "LastSaleDateUtc"              , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "UpdateDateUtc"                , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "QuantityUpdateDateUtc"        , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "ASIN"                         , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Brand"                        , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Condition"                    , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Description"                  , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "EAN"                          , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "FlagDescription"              , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Flag"                         , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "HarmonizedCode"               , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "ISBN"                         , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Manufacturer"                 , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "MPN"                          , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "ShortDescription"             , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Sku"                          , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Subtitle"                     , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "TaxProductCode"               , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Title"                        , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "UPC"                          , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "WarehouseLocation"            , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Warranty"                     , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Height"                       , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Length"                       , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Width"                        , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Weight"                       , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Cost"                         , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Margin"                       , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "RetailPrice"                  , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "StartingPrice"                , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "ReservePrice"                 , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "BuyItNowPrice"                , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "StorePrice"                   , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "SecondChancePrice"            , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "MinPrice"                     , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "MaxPrice"                     , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "SupplierName"                 , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "SupplierCode"                 , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "SupplierPO"                   , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Classification"               , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "IsDisplayInStore"             , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "StoreTitle"                   , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "StoreDescription"             , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "BundleType"                   , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "TotalAvailableQuantity"       , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "OpenAllocatedQuantity"        , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "OpenAllocatedQuantityPooled"  , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "PendingCheckoutQuantity"      , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "PendingCheckoutQuantityPooled", EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "PendingPaymentQuantity"       , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "PendingPaymentQuantityPooled" , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "PendingShipmentQuantity"      , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "PendingShipmentQuantityPooled", EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "TotalQuantity"                , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "TotalQuantityPooled"          , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "MultipackQuantity"            , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Attributes"                   , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "DCQuantities"                 , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Images"                       , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Labels"                       , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "BundleComponents"             , EntityType = KCMappingEntitiesConstants.Product },
                new KCChannelAdvisorMappingField{ FieldName = "Children"                     , EntityType = KCMappingEntitiesConstants.Product },
            };

            return _productFields;
        }
    }
}
