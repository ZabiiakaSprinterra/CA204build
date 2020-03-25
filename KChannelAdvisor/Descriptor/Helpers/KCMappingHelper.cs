using KChannelAdvisor.Descriptor.API.Mapper;
using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using PX.Data;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.Helpers
{
    public class KCMappingHelper
    {
        private List<KCFieldRelation> _orderFields;
        private List<KCFieldRelation> _productFields;

        public List<KCFieldRelation> GetFields(string actionType)
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

        public List<KCFieldRelation> GetOrdersSchema()
        {
            _orderFields = new List<KCFieldRelation>
            {
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Order", AFieldName = "CustomerOrderNbr", CFieldName = "ID"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Order", AFieldName = "CASiteName", CFieldName = "SiteName"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Order", AFieldName = "CustomerRefNbr", CFieldName = "SiteOrderID"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Order", AFieldName = "CuryID", CFieldName = "Currency"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Order", AFieldName = "OrderDate", CFieldName = "CreatedDateUtc"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Order", AFieldName = "CuryPremiumFreightAmt", CFieldName = "TotalShippingPrice"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Order", AFieldName = "CuryOrderTotal", CFieldName = "TotalPrice"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Order", AFieldName = "RequestDate", CFieldName = "EstimatedShipDateUtc"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Order", AFieldName = "ShipDate", CFieldName = "DeliverByDateUtc"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Order", AFieldName = "ExtRefNbr", CFieldName = "PaymentTransactionID"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Order", AFieldName = "CASiteID", CFieldName = "SiteID"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Order", AFieldName = "CAPublicNotes", CFieldName = "PublicNotes"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Order", AFieldName = "CASpecialInstructions", CFieldName = "SpecialInstructions"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Shipping Contact", AFieldName = "Email", CFieldName = "BuyerEmailAddress"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Expression, AViewDisplayName = "Shipping Contact", AFieldName = "Attention", SourceExpression = "ShippingFirstName+ShippingLastName"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Shipping Contact", AFieldName = "FullName", CFieldName = "ShippingCompanyName"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Shipping Contact", AFieldName = "Phone1", CFieldName = "ShippingDaytimePhone"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Shipping Address", AFieldName = "AddressLine1", CFieldName = "ShippingAddressLine1"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Shipping Address", AFieldName = "AddressLine2", CFieldName = "ShippingAddressLine2"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Shipping Address", AFieldName = "City", CFieldName = "ShippingCity"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Shipping Address", AFieldName = "State", CFieldName = "ShippingStateOrProvince"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Shipping Address", AFieldName = "PostalCode", CFieldName = "ShippingPostalCode"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Shipping Address", AFieldName = "CountryID", CFieldName = "ShippingCountry"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Expression, AViewDisplayName = "Billing Contact", AFieldName = "Attention", SourceExpression = "BillingFirstName+BillingLastName"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Billing Contact", AFieldName = "FullName", CFieldName = "BillingCompanyName"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Billing Contact", AFieldName = "Phone1", CFieldName = "BillingDaytimePhone"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Billing Address", AFieldName = "AddressLine1", CFieldName = "BillingAddressLine1"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Billing Address", AFieldName = "AddressLine2", CFieldName = "BillingAddressLine2"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Billing Address", AFieldName = "City", CFieldName = "BillingCity"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Billing Address", AFieldName = "State", CFieldName = "BillingStateOrProvince"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Billing Address", AFieldName = "PostalCode", CFieldName = "BillingPostalCode"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Billing Address", AFieldName = "CountryID", CFieldName = "BillingCountry"},
                new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Order, Direction = KCDirectionsConstants.Import, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Payment Method", AFieldName = "PaymentMethodID", CFieldName = "PaymentMethod"},
            };

            return _orderFields;
        }

        private List<KCFieldRelation> GetProductsSchema()
        {
            _productFields = new List<KCFieldRelation>
            {
               new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Product, Direction = KCDirectionsConstants.Export, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Inventory Item", AFieldName = "InventoryCD", CFieldName = "Sku"},
               new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Product, Direction = KCDirectionsConstants.Export, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Inventory Item", AFieldName = "LastStdCost", CFieldName = "Cost"},
               new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Product, Direction = KCDirectionsConstants.Export, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Inventory Item", AFieldName = "Descr", CFieldName = "Title"},
               new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Product, Direction = KCDirectionsConstants.Export, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Inventory Item", AFieldName = "Body", CFieldName = "Description"},
               new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Product, Direction = KCDirectionsConstants.Export, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Inventory Item", AFieldName = "BaseItemWeight", CFieldName = "Weight"},
               new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Product, Direction = KCDirectionsConstants.Export, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Inventory Item", AFieldName = "BasePrice", CFieldName = "BuyItNowPrice"},
               new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Product, Direction = KCDirectionsConstants.Export, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Inventory Item", AFieldName = "BasePrice", CFieldName = "RetailPrice"},
               //07.11.2019 KA: Removed according new requirement
               //new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Product, Direction = KCDirectionsConstants.Export, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "Inventory Item", AFieldName = "DfltSiteID", CFieldName = "WarehouseLocation"},
               new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Product, Direction = KCDirectionsConstants.Export, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "ChannelAdvisor Item Details", AFieldName = "UsrKCReservePrice", CFieldName = "ReservePrice"},
               new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Product, Direction = KCDirectionsConstants.Export, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "ChannelAdvisor Item Details", AFieldName = "UsrKCStorePrice", CFieldName = "StorePrice"},
               new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Product, Direction = KCDirectionsConstants.Export, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "ChannelAdvisor Item Details", AFieldName = "UsrKCSecondChanceOfferPrice", CFieldName = "SecondChancePrice"},
               new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Product, Direction = KCDirectionsConstants.Export, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "ChannelAdvisor Item Details", AFieldName = "UsrKCProductMargin", CFieldName = "Margin"},
               new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Product, Direction = KCDirectionsConstants.Export, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "ChannelAdvisor Item Details", AFieldName = "UsrKCAllowedForFba", CFieldName = "IsExternalQuantityBlocked"},
               new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Product, Direction = KCDirectionsConstants.Export, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "ChannelAdvisor Item Details", AFieldName = "UsrKCStartingPrice", CFieldName = "StartingPrice"},
               new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Product, Direction = KCDirectionsConstants.Export, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "ChannelAdvisor Item Details", AFieldName = "UsrKCMaxPrice", CFieldName = "MaxPrice"},
               new KCFieldRelation{ EntityType = KCMappingEntitiesConstants.Product, Direction = KCDirectionsConstants.Export, MappingRule = KCMappingRuleConstants.Static, RuleType = KCRuleTypesConstants.Simple, AViewDisplayName = "ChannelAdvisor Item Details", AFieldName = "UsrKCMinPrice", CFieldName = "MinPrice"},
            };

            return _productFields;
        }
    }
}
