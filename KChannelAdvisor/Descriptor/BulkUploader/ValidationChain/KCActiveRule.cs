using KChannelAdvisor.BLC;
using KChannelAdvisor.Descriptor;
using PX.Objects.IN;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.Descriptor.BulkUploader.ValidationChain
{
    internal class KCActiveRule : KCAbstractRule
    {
        private KCBulkProductMaint Graph { get;}

        public KCActiveRule(KCBulkProductMaint graph)
        {
            Graph = graph;
        }

        public override IEnumerable<InventoryItem> Validate(IEnumerable<InventoryItem> inventoryItems)
        {
            // 06/05/19 AT: Added .ToList() call in order to prevent these validations being deferred 
            // thus calling these validation rules twice.
            var validList = inventoryItems.Where(IsActive).ToList();

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
        /// Check whether InventoryItem has status Active
        /// </summary>
        /// <param name="inventoryItem">InventoryItem that should be checked</param>
        /// <returns>Validation result</returns>
        private bool IsActive(InventoryItem inventoryItem)
        {
            if (inventoryItem.KitItem == true)
            {
                INKitSpecHdr kitSpecification = Graph.KitProduct.SelectSingle(inventoryItem.InventoryID);
                return kitSpecification?.IsActive == true && inventoryItem.ItemStatus.Equals(KCConstants.Active);
            }
            else
            {
                return inventoryItem.ItemStatus.Equals(KCConstants.Active);
            }
        }
    }
}