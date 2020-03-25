using KChannelAdvisor.BLC;
using KChannelAdvisor.Descriptor;
using ProductConfigurator.DAC;
using ProductConfigurator.DAC.Ext;
using PX.Data;
using PX.Objects.IN;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.Descriptor.BulkUploader.ValidationChain
{
    internal class KCValidChildrenRule : KCAbstractRule
    {
        private IEnumerable<InventoryItem> InputList { get; set; }
        private KCBulkProductMaint Graph { get; set; }

        public KCValidChildrenRule(KCBulkProductMaint graph)
        {
            Graph = graph;
        }

        public override IEnumerable<InventoryItem> Validate(IEnumerable<InventoryItem> inventoryItems)
        {
            InputList = inventoryItems;
            // 06/05/19 AT: Added .ToList() call in order to prevent these validations being deferred 
            // thus calling these validation rules twice.
            var validList = InputList.Where(x => ValidateChildren(x)).ToList();
            InputList = validList;
            validList = InputList.Where(x => RemoveInvalidChildren(x)).ToList();

            if (validList.Count == 0)
            {
                return validList;
            }
            else
            {
                return base.Validate(validList);
            }
        }

        private bool RemoveInvalidChildren(InventoryItem item)
        {
            InventoryItemPCExt itemPCExt = item.GetExtension<InventoryItemPCExt>();
            string parentCD = null;
            bool result = true;

            if (itemPCExt != null && itemPCExt.UsrKNCompositeID != null)
            {
                parentCD = Graph.ItemById.SelectSingle(itemPCExt.UsrKNCompositeID).InventoryCD;
            }

            if (parentCD != null)
            {
                result = InputList.Any(x => x.InventoryCD == parentCD);
                if (!result) return result;
            }

            if (item.StkItem.GetValueOrDefault())
            {
                PXResultset<INKitSpecStkDet> stockComponents = Graph.StockKitComponentsExisted.Select(item.InventoryID);
                foreach (INKitSpecStkDet comp in stockComponents)
                {
                    string revisionId = Graph.KitProduct.SelectSingle(comp.KitInventoryID)?.RevisionID;
                    if (comp.RevisionID == revisionId)
                    result = InputList.Any(x => x.InventoryID == comp.KitInventoryID.Value);
                    if (result) return result;
                }
            }
            else
            {
                PXResultset<INKitSpecNonStkDet> nonStockComponents = Graph.NonStockKitComponentsExisted.Select(item.InventoryID);
                foreach (INKitSpecNonStkDet comp in nonStockComponents)
                {
                    string revisionId = Graph.KitProduct.SelectSingle(comp.KitInventoryID)?.RevisionID;
                    if (comp.RevisionID == revisionId)
                    result = InputList.Any(x => x.InventoryID == comp.KitInventoryID.Value);
                    if (result) return result;
                }
            }
            return result;
        }

        /// <summary>
        /// Check whether all Grouped/Configurable products'/Kits' children are valid.
        /// </summary>
        /// <param name="inventoryItem">Grouped/Configurable Product/Kit that should be checked</param>
        /// <returns>Validation result</returns>
        private bool ValidateChildren(InventoryItem inventoryItem)
        {
            List<int> validIds = InputList.Select(validItem => validItem.InventoryID.GetValueOrDefault()).ToList();
            List<int> childIds = new List<int>();
            bool result = false;
            InventoryItemPCExt itemPCExt = inventoryItem.GetExtension<InventoryItemPCExt>();

            if (itemPCExt != null)
            {
                if (itemPCExt.UsrKNCompositeType == KCConstants.GroupedProduct)
                {
                    IEnumerable<KNSIGroupedItems> childItems = Graph.GroupedItemChilds.Select(inventoryItem.InventoryID).RowCast<KNSIGroupedItems>();
                    childIds.AddRange(childItems.Select(child => child.MappedInventoryID.GetValueOrDefault()));
                }
                else if (itemPCExt.UsrKNCompositeType == KCConstants.ConfigurableProduct)
                { 
                    IEnumerable<InventoryItem> childItems = Graph.ChildrenByCompositeId.Select(inventoryItem.InventoryID).RowCast<InventoryItem>();
                    childIds.AddRange(childItems.Select(child => child.InventoryID.GetValueOrDefault()));
                }
            }
            if (inventoryItem.KitItem.GetValueOrDefault())
            {
                string revisionId = Graph.KitProduct.SelectSingle(inventoryItem.InventoryID)?.RevisionID;
                IEnumerable<INKitSpecStkDet> stockKitComponents = Graph.StockKitComponents.Select(inventoryItem.InventoryID, revisionId)?.RowCast<INKitSpecStkDet>();
                IEnumerable<INKitSpecNonStkDet> nonStockKitComponents = Graph.NonStockKitComponents.Select(inventoryItem.InventoryID, revisionId)?.RowCast<INKitSpecNonStkDet>();
                childIds.AddRange(stockKitComponents.Select(x => x.CompInventoryID.GetValueOrDefault()));
                childIds.AddRange(nonStockKitComponents.Select(x => x.CompInventoryID.GetValueOrDefault()));
            }

            result = childIds.Count == 0 || childIds.All(child => validIds.Contains(child));
            return result;
        }
    }
}
