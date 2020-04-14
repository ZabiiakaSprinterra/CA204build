using KChannelAdvisor.BLC;
using KChannelAdvisor.Descriptor.BulkUploader.Strategy.Interfaces;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor.Extensions;
using ProductConfigurator.DAC.Ext;
using PX.Data;
using PX.Objects.IN;
using System.Collections.Generic;
using System;

namespace KChannelAdvisor.Descriptor.BulkUploader.Strategy
{
    public class KCDeltaSync : KCBasicBulkStrategy, IKCBulkStrategy
    {
        public KCDeltaSync(KCBulkProductMaint graph) : base(graph)
        {
            ImagePlacements = new PXSelect<KCImagePlacement,
                Where<KCImagePlacement.lastModifiedDateTime, Greater<Required<KNSIKCInventoryItem.usrKCCASyncDate>>>>(graph);

            AttributesMappings = new PXSelect<KCAttributesMapping,
                Where<KCAttributesMapping.lastModifiedDateTime, Greater<Required<KNSIKCInventoryItem.usrKCCASyncDate>>>>(graph);

            ClassificationMappings = new PXSelect<KNSIKCClassificationsMapping,
                Where<KNSIKCClassificationsMapping.lastModifiedDateTime, Greater<Required<KNSIKCInventoryItem.usrKCCASyncDate>>>>(graph);

            RequiredRelation = new PXSelect<KNSIKCRelationship,
                Where<KNSIKCRelationship.itemClassId, Equal<Required<InventoryItem.itemClassID>>,
                And<KNSIKCRelationship.lastModifiedDateTime, Greater<Required<KNSIKCInventoryItem.usrKCCASyncDate>>>>>(graph);

            KitProduct = new PXSelect<INKitSpecHdr,
                Where<INKitSpecHdr.kitInventoryID, Equal<Required<INKitSpecHdr.kitInventoryID>>>>(graph);

            StockKitComponents = new PXSelect<INKitSpecStkDet,
                Where<INKitSpecStkDet.kitInventoryID, Equal<Required<INKitSpecStkDet.kitInventoryID>>,
                    And<INKitSpecStkDet.revisionID, Equal<Required<INKitSpecStkDet.revisionID>>,
                        And<Where<INKitSpecStkDet.lastModifiedDateTime, Greater<Required<KNSIKCInventoryItem.usrKCCASyncDate>>>>>>>(graph);

            NonStockKitComponents = new PXSelect<INKitSpecNonStkDet,
                Where<INKitSpecNonStkDet.kitInventoryID, Equal<Required<INKitSpecNonStkDet.kitInventoryID>>,
                    And<INKitSpecNonStkDet.revisionID, Equal<Required<INKitSpecNonStkDet.revisionID>>,
                        And<Where<INKitSpecNonStkDet.lastModifiedDateTime, Greater<Required<KNSIKCInventoryItem.usrKCCASyncDate>>>>>>>(graph);

            QuantityUpdate = new PXSelect<INTran,
                Where<INTran.lastModifiedDateTime, Greater<Required<KNSIKCInventoryItem.usrKCCASyncDate>>>>(graph);
        }

        protected override List<string> GetAllowedHeaders()
        {
            // Add additional properties to the list
            List<string> extendedHeadersList = new List<string>
            {
                KCHeaders.QuantityUpdateType        ,
                KCHeaders.Quantity                  ,
                KCHeaders.DCQuantity                ,
                KCHeaders.DCQuantityUpdateType      ,
                KCHeaders.StartingBid               ,
                KCHeaders.Reserve                   ,
                KCHeaders.SellerCost                ,
                KCHeaders.EstimatedShippingCost     ,
                KCHeaders.ProductMargin             ,
                KCHeaders.BuyItNowPrice             ,
                KCHeaders.RetailPrice               ,
                KCHeaders.SecondChanceOfferPrice    ,
                KCHeaders.MinimumPrice              ,
                KCHeaders.MaximumPrice              ,
                KCHeaders.MultipackQuantity         ,
                KCHeaders.StorePrice                ,
            };
            // Merge basic version of headers list with additional one.
            extendedHeadersList.AddRange(base.GetAllowedHeaders());

            return extendedHeadersList;
        }
        #region Update Validation
        /// <summary>
        /// Checks if inventory item was updated
        /// </summary>
        /// <param name="result">PXResult&lt;InventoryItem, KCInventoryItem&gt;</param>
        protected override bool IsUpdated(PXResult<InventoryItem> result)
        {
            var product = result.GetItem<InventoryItem>();
            var kcProduct = result.GetItem<KNSIKCInventoryItem>();

            if (product == null || kcProduct == null)
            {
                throw new ArgumentException($"{nameof(result)} must contain {nameof(InventoryItem)} and {nameof(KNSIKCInventoryItem)}");
            }

            return kcProduct.UsrKCCASyncDate == null
                   || IsProductUpdated(product, kcProduct)
                   || IsKcProductUpdated(kcProduct)
                   || IsImagePlacementUpdated(kcProduct)
                   || IsAttributesMappingUpdated(kcProduct)
                   || IsSiteMasterUpdated(kcProduct)
                   || IsClassificationUpdated(kcProduct)
                   || IsVariationRelationshipUpdated(product, kcProduct)
                   || IsKitUpdated(product, kcProduct)
                   || IsQuantityUpdated(kcProduct);
        }

        private bool IsProductUpdated(InventoryItem product, KNSIKCInventoryItem kcProduct)
        {
            return product.LastModifiedDateTime.GetValueOrDefault().BiggerThan(kcProduct.UsrKCCASyncDateTicks);
        }

        private bool IsKcProductUpdated(KNSIKCInventoryItem kcProduct)
        {
            return kcProduct.LastModifiedDateTime.GetValueOrDefault().BiggerThan(kcProduct.UsrKCCASyncDateTicks);
        }

        private bool IsImagePlacementUpdated(KNSIKCInventoryItem kcProduct)
        {
            return ImagePlacements.SelectSingle(kcProduct.UsrKCCASyncDate) != null;
        }

        private bool IsQuantityUpdated(KNSIKCInventoryItem kcProduct)
        {
            return QuantityUpdate.SelectSingle(kcProduct.UsrKCCASyncDate) != null;
        }

        private bool IsAttributesMappingUpdated(KNSIKCInventoryItem kcProduct)
        {
            return AttributesMappings.SelectSingle(kcProduct.UsrKCCASyncDate) != null;
        }

        private bool IsSiteMasterUpdated(KNSIKCInventoryItem kcProduct)
        {
            return SiteMaster.LastModifiedDateTime > kcProduct.UsrKCCASyncDate.GetValueOrDefault();
        }

        private bool IsClassificationUpdated(KNSIKCInventoryItem productKcExt)
        {
            return ClassificationMappings.SelectSingle(productKcExt.UsrKCCASyncDate) != null;
        }

        private bool IsVariationRelationshipUpdated(InventoryItem product, KNSIKCInventoryItem kcProduct)
        {
            InventoryItemPCExt productPcExt = product.GetExtension<InventoryItemPCExt>();
            if (productPcExt?.UsrKNCompositeType != KCConstants.ConfigurableProduct)
            {
                return false;
            }

            return RequiredRelation.SelectSingle(product.ItemClassID, kcProduct.UsrKCCASyncDate) != null;
        }

        private bool IsKitUpdated(InventoryItem product, KNSIKCInventoryItem kcProduct)
        {
            if (product.KitItem.GetValueOrDefault())
            {
                if (kcProduct == null)
                {
                    return true;
                }

                var kitProduct = KitProduct.SelectSingle(product.InventoryID);
                if (kitProduct == null)
                {
                    return false;
                }

                if (kitProduct.LastModifiedDateTime.GetValueOrDefault().BiggerThan(kcProduct.UsrKCCASyncDate.GetValueOrDefault()))
                {
                    return true;
                }

                if (StockKitComponents.SelectSingle(product.InventoryID, kitProduct.RevisionID, kcProduct.UsrKCCASyncDate) != null)
                {
                    return true;
                }

                if (NonStockKitComponents.SelectSingle(product.InventoryID, kitProduct.RevisionID, kcProduct.UsrKCCASyncDate) != null)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
