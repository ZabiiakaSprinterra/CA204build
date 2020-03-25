using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.Descriptor.API.APIHelper;
using KChannelAdvisor.Descriptor.API.DataHelper;
using KChannelAdvisor.BLC;
using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor.Logger;
using KChannelAdvisor.Descriptor.MSMQ.Models;
using ProductConfigurator.DAC;
using ProductConfigurator.DAC.Ext;
using PX.Data;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.Descriptor.API.Mapper
{
    public class KCMapInventoryItem
    {
        private KCStore Store { get; }
        private KCRelationshipSetupMaint RelationshipGraph { get; }
        private KCClassificationsMappingMaint ClassificationsGraph { get; }
        private KCBulkProductMaint BulkGraph { get; }
        private KCIItemConversionDataMaint ConversionGraph { get; }
        private KCLoggerProperties LoggerProperties { get; }

        public KCMapInventoryItem(KCRelationshipSetupMaint relationshipGraph,
            KCClassificationsMappingMaint classificationsGraph,
            KCBulkProductMaint bulkGraph,
            KCIItemConversionDataMaint conversionGraph,
            KCStore store,
            KCLoggerProperties loggerProperties)
        {
            RelationshipGraph = relationshipGraph;
            ClassificationsGraph = classificationsGraph;
            BulkGraph = bulkGraph;
            ConversionGraph = conversionGraph;
            Store = store;
            LoggerProperties = loggerProperties;
        }

        public KCAPIInventoryItem GetAPIInventoryItem(InventoryItem product)
        {
            DAC.KNSIKCInventoryItem kcProduct = BulkGraph.KCInventoryItem.SelectSingle(product.InventoryID);

            InventoryItemPCExt productPcExt = product.GetExtension<InventoryItemPCExt>();
            string classificationName = KCGeneralDataHelper.GetClassificationByInventoryId(ClassificationsGraph, product);

            KCAPIInventoryItem apiProduct = new KCAPIInventoryItem()
            {
                Classification = classificationName
            };

            if (IsConfigurableParentOrChild(product, productPcExt))
                PropagateConfigurable(product, apiProduct, productPcExt.UsrKNCompositeType == null);
            if (productPcExt.UsrKNCompositeType == KCConstants.GroupedProduct)
                PropagateBundle(product, apiProduct);
            if (product.KitItem.GetValueOrDefault())
                PropagateKit(product, apiProduct);

            KCDynamicProductMapper mapper = new KCDynamicProductMapper(KCMappingEntitiesConstants.Product);
            mapper.Mapping.MappingValues = ConversionGraph.GetEntity(product.InventoryCD);
            apiProduct = mapper.MapApiInventoryItem(apiProduct, product, kcProduct);
            apiProduct = KCGeneralDataHelper.FillReservedAttributes(product, apiProduct);

            return apiProduct;
        }

        public void PropagateKit(InventoryItem product, KCAPIInventoryItem apiProduct)
        {
            List<KCAPIBundleComponent> bundleComponents = new List<KCAPIBundleComponent>();

            INKitSpecHdr kit = BulkGraph.KitProduct.SelectSingle(product.InventoryID);

            foreach (INKitSpecStkDet component in BulkGraph.StockKitComponents.Select(product.InventoryID, kit.RevisionID))
            {
                if(!ComponentIsActive(component.CompInventoryID)) continue;

                bundleComponents.Add(new KCAPIBundleComponent()
                {
                    ComponentSku = BulkGraph.ItemById.SelectSingle(component.CompInventoryID).InventoryCD.Trim(),
                    Quantity = Convert.ToInt32(component.DfltCompQty)
                });
            }

            foreach (INKitSpecNonStkDet component in BulkGraph.NonStockKitComponents.Select(product.InventoryID, kit.RevisionID))
            {
                if (!ComponentIsActive(component.CompInventoryID)) continue;

                bundleComponents.Add(new KCAPIBundleComponent()
                {
                    ComponentSku = BulkGraph.ItemById.SelectSingle(component.CompInventoryID).InventoryCD.Trim(),
                    Quantity = Convert.ToInt32(component.DfltCompQty)
                });
            }

            DeleteObsoleteBundleComponents(bundleComponents, product);

            apiProduct.BundleType = "BundleItem";
            apiProduct.BundleComponents = bundleComponents;
        }

        private bool ComponentIsActive(int? id)
        {
            var kcInventoryItem = BulkGraph.KCInventoryItem.SelectSingle(id);
            return kcInventoryItem != null && kcInventoryItem.UsrKCActiveOnCa.GetValueOrDefault();
        }

        public void PropagateBundle(InventoryItem product, KCAPIInventoryItem apiProduct)
        {
            List<KCAPIBundleComponent> bundleComponents = new List<KCAPIBundleComponent>();

            foreach (KNSIGroupedItems component in BulkGraph.GroupedItemChilds.Select(product.InventoryID))
            {
                bundleComponents.Add(new KCAPIBundleComponent()
                {
                    ComponentSku = BulkGraph.ItemById.SelectSingle(component.MappedInventoryID).InventoryCD.Trim(),
                    Quantity = Convert.ToInt32(component.Quantity)
                });
            }

            DeleteObsoleteBundleComponents(bundleComponents, product);

            apiProduct.BundleType = "BundleItem";
            apiProduct.BundleComponents = bundleComponents;
        }

        public void PropagateConfigurable(InventoryItem product, KCAPIInventoryItem apiProduct, bool isChild)
        {
            apiProduct.VariationParentSku = isChild ? BulkGraph.ItemById.SelectSingle(product.GetExtension<InventoryItemPCExt>().UsrKNCompositeID).InventoryCD
                                                    : KCConstants.Parent;
            string relationshipName = KCGeneralDataHelper.GetRelationshipName(RelationshipGraph, product.ItemClassID);
            apiProduct.RelationshipName = relationshipName;
        }

        public static KCAPIBundleComponent GetAPIGroupedBundleComponent(KCInventoryItemAPIHelper helper, int? productId, KNSIGroupedItems component)
        {
            KCDataExchangeMaint graph = PXGraph.CreateInstance<KCDataExchangeMaint>();
            return new KCAPIBundleComponent()
            {
                ProductID = productId.GetValueOrDefault(),
                ComponentID = KCGeneralDataHelper.GetCAIDByInventoryId(graph, helper, component.MappedInventoryID).GetValueOrDefault(),
                Quantity = Convert.ToInt32(component.Quantity)
            };
        }

        public static APIQuantityValue GetAPIQuantity(int? inventoryID, List<INLocationStatus> statuses = null, decimal? vendorQty = null)
        {
            KCBulkProductMaint graph = PXGraph.CreateInstance<KCBulkProductMaint>();
            KCInventoryManagementMaint inventoryManagementGraph = PXGraph.CreateInstance<KCInventoryManagementMaint>();
            if (statuses == null)
            {
                var settingsGraph = PXGraph.CreateInstance<KCInventoryManagementMaint>();
                statuses = KCGeneralDataHelper.GetINLocationStatuses(graph, inventoryID).Where(x => settingsGraph.IsSiteMapped(x.SiteID)).ToList();
            }
            if (vendorQty == null) vendorQty = KCGeneralDataHelper.GetVendorQty(inventoryID);

            List<KCAPIQuantity> qtys = KCGeneralDataHelper.GetProductQtys(inventoryManagementGraph, inventoryID, statuses, vendorQty);

            return new APIQuantityValue
            {
                Value = new APIUpdates()
                {
                    UpdateType = "InStock",
                    Updates = qtys
                }
            };
        }

        public static KCAPIInventoryItem GetAPIMSMQInventoryPrice(KCMSMQInventoryPrice product)
        {
            return new KCAPIInventoryItem()
            {
                BuyItNowPrice = (decimal)Math.Round(Convert.ToDouble(product.DefaultPrice), 2),
                RetailPrice = (decimal)Math.Round(Convert.ToDouble(product.RetailPrice), 2),
                StartingPrice = (decimal)Math.Round(Convert.ToDouble(product.StartingPrice), 2),
                ReservePrice = (decimal)Math.Round(Convert.ToDouble(product.ReservePrice), 2),
                StorePrice = (decimal)Math.Round(Convert.ToDouble(product.StorePrice), 2),
                SecondChancePrice = (decimal)Math.Round(Convert.ToDouble(product.SecondChanceOfferPrice), 2),
                Margin = (decimal)Math.Round(Convert.ToDouble(product.ProductMargin), 2),
                MinPrice = (decimal)Math.Round(Convert.ToDouble(product.MinimumPrice), 2),
                MaxPrice = (decimal)Math.Round(Convert.ToDouble(product.MaximumPrice), 2)
            };
        }

        public void DeleteObsoleteBundleComponents(List<KCAPIBundleComponent> bundleComponents, InventoryItem product)
        {
            var caid = BulkGraph.KCInventoryItem.SelectSingle(product.InventoryID)?.UsrKCCAID;
            if (caid.GetValueOrDefault() < 1) return;
            KCDataExchangeMaint graph = PXGraph.CreateInstance<KCDataExchangeMaint>();
            KCSiteMaster connection = graph.Connection.SelectSingle();
            KCARestClient client = new KCARestClient(connection);
            KCInventoryItemAPIHelper helper = new KCInventoryItemAPIHelper(client, LoggerProperties);
            KCODataWrapper<KCAPIBundleComponent> response = helper.GetBundleComponents(caid);
            if (response == null) return;
            List<KCAPIBundleComponent> cBundleComponents = helper.GetBundleComponents(caid).Value.ToList();

            foreach (KCAPIBundleComponent component in cBundleComponents.Where(x => bundleComponents.All(y => y.ComponentSku != x.ComponentSku)))
            {
                helper.DeleteBundleComponent(caid, component.ComponentID, product.InventoryCD, component.ComponentID.ToString());
            }
        }

        private bool IsConfigurableParentOrChild(InventoryItem product, InventoryItemPCExt productPcExt)
        {
            return (productPcExt.UsrKNCompositeType == null
                        && productPcExt.UsrKNCompositeID != null
                        && BulkGraph.ItemById.SelectSingle(productPcExt.UsrKNCompositeID).GetExtension<InventoryItemPCExt>().UsrKNCompositeType == KCConstants.ConfigurableProduct)
                    || productPcExt.UsrKNCompositeType == KCConstants.ConfigurableProduct;
        }
    }
}
