using KChannelAdvisor.BLC;
using ProductConfigurator.DAC.Ext;
using PX.Data;
using PX.Objects.IN;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.Descriptor.BulkUploader.ValidationChain
{
    class KCValidParentRule : KCAbstractRule
    {
        private IEnumerable<InventoryItem> InputList { get; set; }
        private KCBulkProductMaint Graph { get; set; }

        public KCValidParentRule(KCBulkProductMaint graph)
        {
            Graph = graph;
        }

        public override IEnumerable<InventoryItem> Validate(IEnumerable<InventoryItem> inventoryItems)
        {
            InputList = inventoryItems;
            // 06/05/19 AT: Added .ToList() call in order to prevent these validations being deferred 
            // thus calling these validation rules twice.
            var validList = InputList.Where(x => Validate(x)).ToList();

            if (validList.Count == 0)
            {
                return validList;
            }
            else
            {
                return base.Validate(validList);
            }
        }

        /// <summary>
        /// Check whether parent of composite child item is valid.
        /// </summary>
        /// <param name="inventoryItem">Grouped/Configurable Product/Kit that should be checked</param>
        /// <returns>Validation result</returns>
        private bool Validate(InventoryItem inventoryItem)
        {
            List<int> validIds = InputList.Select(validItem => validItem.InventoryID.GetValueOrDefault()).ToList();
            //List<int> childIds = new List<int>();
            bool result = true;
            InventoryItemPCExt itemPCExt = inventoryItem.GetExtension<InventoryItemPCExt>();

            if (itemPCExt != null && itemPCExt.UsrKNCompositeType == null)
            {
                if (itemPCExt.UsrKNCompositeID != null)
                {
                    result = validIds.Contains(itemPCExt.UsrKNCompositeID.Value);
                    if (!result) return result;
                }

                if (inventoryItem.StkItem.GetValueOrDefault())
                {
                    PXResultset<INKitSpecStkDet> stockComponents = Graph.StockKitComponentsExisted.Select(inventoryItem.InventoryID);
                    foreach(INKitSpecStkDet comp in stockComponents)
                    {
                        string revisionId = Graph.KitProduct.SelectSingle(comp.KitInventoryID)?.RevisionID;
                        if (comp.RevisionID == revisionId)
                        result = validIds.Contains(comp.KitInventoryID.Value);
                        if (result) return result;
                    }
                }
                else
                {
                    PXResultset<INKitSpecNonStkDet> nonStockComponents = Graph.NonStockKitComponentsExisted.Select(inventoryItem.InventoryID);
                    foreach (INKitSpecNonStkDet comp in nonStockComponents)
                    {
                        string revisionId = Graph.KitProduct.SelectSingle(comp.KitInventoryID)?.RevisionID;
                        if (comp.RevisionID == revisionId)
                            result = validIds.Contains(comp.KitInventoryID.Value);
                        if (result) return result;
                    }
                }
            }
            
            return result;
        }
    }
}
