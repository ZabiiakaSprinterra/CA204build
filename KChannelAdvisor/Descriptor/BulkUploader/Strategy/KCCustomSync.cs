using KChannelAdvisor.BLC;
using KChannelAdvisor.Descriptor.BulkUploader.Strategy.Interfaces;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor.Extensions;
using ProductConfigurator.DAC.Ext;
using PX.Data;
using PX.Objects.IN;
using System;

namespace KChannelAdvisor.Descriptor.BulkUploader.Strategy
{
    public class KCCustomSync : KCBasicBulkStrategy, IKCBulkStrategy
    {
        private readonly DateTime DateFrom;
        private readonly DateTime DateTo;


        public KCCustomSync(KCBulkProductMaint graph, DateTime dateFrom, DateTime dateTo) : base(graph)
        {
            DateFrom = dateFrom.AddHours(-0);
            DateTo = dateTo.AddHours(11);
            if (dateFrom.Date == DateTime.Now.Date)
            {
                DateTo = dateTo.AddHours(24);
            }
            ImagePlacements = new PXSelect<KCImagePlacement,
                Where<KCImagePlacement.lastModifiedDateTime, Greater<Required<KCImagePlacement.lastModifiedDateTime>>,
                    And<Where<KCImagePlacement.lastModifiedDateTime, Less<Required<KCImagePlacement.lastModifiedDateTime>>>>>>(graph);

            AttributesMappings = new PXSelect<KCAttributesMapping,
                Where<KCAttributesMapping.lastModifiedDateTime, Greater<Required<KCAttributesMapping.lastModifiedDateTime>>,
                    And<Where<KCAttributesMapping.lastModifiedDateTime, Less<Required<KCAttributesMapping.lastModifiedDateTime>>>>>>(graph);

            ClassificationMappings = new PXSelect<KNSIKCClassificationsMapping,
                Where<KNSIKCClassificationsMapping.lastModifiedDateTime, Greater<Required<KNSIKCClassificationsMapping.lastModifiedDateTime>>,
                    And<Where<KNSIKCClassificationsMapping.lastModifiedDateTime, Less<Required<KNSIKCClassificationsMapping.lastModifiedDateTime>>>>>>(graph);

            RequiredRelation = new PXSelect<KNSIKCRelationship,
                Where<KNSIKCRelationship.itemClassId, Equal<Required<InventoryItem.itemClassID>>,
                    And<KNSIKCRelationship.lastModifiedDateTime, Greater<Required<KNSIKCRelationship.lastModifiedDateTime>>,
                        And<Where<KNSIKCRelationship.lastModifiedDateTime, Less<Required<KNSIKCRelationship.lastModifiedDateTime>>>>>>>(graph);

            KitProduct = new PXSelect<INKitSpecHdr,
                Where<INKitSpecHdr.kitInventoryID, Equal<Required<INKitSpecHdr.kitInventoryID>>>>(graph);

            StockKitComponents = new PXSelect<INKitSpecStkDet,
                Where<INKitSpecStkDet.kitInventoryID, Equal<Required<INKitSpecStkDet.kitInventoryID>>,
                    And<INKitSpecStkDet.revisionID, Equal<Required<INKitSpecStkDet.revisionID>>,
                        And<Where<INKitSpecStkDet.lastModifiedDateTime, Greater<Required<INKitSpecStkDet.lastModifiedDateTime>>,
                            And<Where<INKitSpecStkDet.lastModifiedDateTime, Less<Required<INKitSpecStkDet.lastModifiedDateTime>>>>>>>>>(graph);

            NonStockKitComponents = new PXSelect<INKitSpecNonStkDet,
                Where<INKitSpecNonStkDet.kitInventoryID, Equal<Required<INKitSpecNonStkDet.kitInventoryID>>,
                    And<INKitSpecNonStkDet.revisionID, Equal<Required<INKitSpecNonStkDet.revisionID>>,
                        And<Where<INKitSpecNonStkDet.lastModifiedDateTime, Greater<Required<INKitSpecNonStkDet.lastModifiedDateTime>>,
                            And<INKitSpecNonStkDet.lastModifiedDateTime, Less<Required<INKitSpecNonStkDet.lastModifiedDateTime>>>>>>>>(graph);

            QuantityUpdate = new PXSelect<INTran,
              Where<INTran.lastModifiedDateTime, Greater<Required<KNSIKCInventoryItem.usrKCCASyncDate>>>>(graph);
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
            bool res = IsProductUpdated(product);
                   //|| IsKcProductUpdated(kcProduct);
                   //|| IsImagePlacementUpdated()
                   //|| IsAttributesMappingUpdated()
                   //|| IsSiteMasterUpdated()
                   //|| IsClassificationUpdated()
                   //|| IsVariationRelationshipUpdated(product)
                   //|| IsKitUpdated(product, kcProduct)
                   //|| IsQuantityUpdated(kcProduct);
            return res;
        }

        private bool IsProductUpdated(InventoryItem product)
        {
            return product.LastModifiedDateTime.GetValueOrDefault().BiggerThan(DateFrom) && product.LastModifiedDateTime.GetValueOrDefault().LessThan(DateTo);
        }

        private bool IsKcProductUpdated(KNSIKCInventoryItem kcProduct)
        {
            return kcProduct.LastModifiedDateTime.GetValueOrDefault().BiggerThan(DateFrom) && kcProduct.LastModifiedDateTime.GetValueOrDefault().LessThan(DateTo);
        }

        private bool IsQuantityUpdated(KNSIKCInventoryItem kcProduct)
        {
            return QuantityUpdate.SelectSingle(kcProduct.UsrKCCASyncDate) != null;
        }

        private bool IsImagePlacementUpdated()
        {
            return ImagePlacements.SelectSingle(DateFrom, DateTo) != null;
        }

        private bool IsAttributesMappingUpdated()
        {
            return AttributesMappings.SelectSingle(DateFrom, DateTo) != null;
        }

        private bool IsSiteMasterUpdated()
        {
            return SiteMaster.LastModifiedDateTime > DateFrom && SiteMaster.LastModifiedDateTime <= DateTo;
        }

        private bool IsClassificationUpdated()
        {
            return ClassificationMappings.SelectSingle(DateFrom, DateTo) != null;
        }

        private bool IsVariationRelationshipUpdated(InventoryItem product)
        {
            InventoryItemPCExt productPcExt = product.GetExtension<InventoryItemPCExt>();
            if (productPcExt.UsrKNCompositeType != KCConstants.ConfigurableProduct)
            {
                return false;
            }

            return RequiredRelation.SelectSingle(product.ItemClassID, DateFrom, DateTo) != null;
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

                var kitLastModifiedTime = kitProduct.LastModifiedDateTime.GetValueOrDefault();
                if (kitLastModifiedTime.BiggerThan(DateFrom) && kitLastModifiedTime.LessThan(DateTo))
                {
                    return true;
                }

                if (StockKitComponents.SelectSingle(product.InventoryID, kitProduct.RevisionID, DateFrom, DateTo) != null)
                {
                    return true;
                }

                if (NonStockKitComponents.SelectSingle(product.InventoryID, kitProduct.RevisionID, DateFrom, DateTo) != null)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
