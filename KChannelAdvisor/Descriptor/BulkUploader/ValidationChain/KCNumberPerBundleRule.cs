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
    /// <summary>
    /// This Rule should be the last in the validation chain in order to have list of items
    /// that went though all validations, so that we can properly check Grouped Product's children
    /// </summary>
    internal class KCNumberPerBundleRule : KCAbstractRule
    {
        private IEnumerable<InventoryItem> InputList { get; set; }
        private KCBulkProductMaint Graph { get; set; }

        public KCNumberPerBundleRule(KCBulkProductMaint graph)
        {
            Graph = graph;
        }

        public override IEnumerable<InventoryItem> Validate(IEnumerable<InventoryItem> inventoryItems)
        {
            InputList = inventoryItems;
            // 06/05/19 AT: Added .ToList() call in order to prevent these validations being deferred 
            // thus calling these validation rules twice.
            var validList = inventoryItems.Where(x => HasMoreThanOneQuantity(x)).ToList();

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
        /// Check whether sum of Quantities of the Grouped Items' and Kits' children is bigger than 1.
        /// </summary>
        /// <param name="inventoryItem">InventoryItem that should be checked</param>
        /// <returns>Validation result</returns>
        private bool HasMoreThanOneQuantity(InventoryItem inventoryItem)
        {
            decimal sum = 2;
            InventoryItemPCExt itemPcExt = inventoryItem.GetExtension<InventoryItemPCExt>();

            if (itemPcExt?.UsrKNCompositeType == KCConstants.GroupedProduct)
            {
                IEnumerable<KNSIGroupedItems> children = Graph.GroupedItemChilds.Select(inventoryItem.InventoryID)?.RowCast<KNSIGroupedItems>();
                // 06/05/19 AT: InputList contains only those items, that are valid for ChannelAdvisor(excluding this rule).
                //              We need to take into account only those children, which are valid for Export.
                //              The intersection of children and InputList will give us the desired list.
                children = children.Where(child => InputList.Any(validatedItems => validatedItems.InventoryID == child.MappedInventoryID));
                sum = children != null ? children.Sum(x => x.Quantity.GetValueOrDefault()) : 0;
            }
            else if (inventoryItem.KitItem.GetValueOrDefault())
            {
                string revisionId = Graph.KitProduct.SelectSingle(inventoryItem.InventoryID)?.RevisionID;
                sum = (decimal)Graph.StockKitComponents.Select(inventoryItem.InventoryID, revisionId)?.RowCast<INKitSpecStkDet>()?.Sum(x => x.DfltCompQty.GetValueOrDefault());
                sum += (decimal)Graph.NonStockKitComponents.Select(inventoryItem.InventoryID, revisionId)?.RowCast<INKitSpecNonStkDet>()?.Sum(x => x.DfltCompQty.GetValueOrDefault());
            }

            return sum > 1;
        }
    }
}
