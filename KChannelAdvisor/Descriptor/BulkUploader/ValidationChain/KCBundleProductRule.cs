using KChannelAdvisor.Descriptor;
using ProductConfigurator.DAC.Ext;
using PX.Data;
using PX.Objects.IN;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.Descriptor.BulkUploader.ValidationChain
{
    internal class KCBundleProductRule : KCAbstractRule
    {
        public override IEnumerable<InventoryItem> Validate(IEnumerable<InventoryItem> inventoryItems)
        {
            // 06/05/19 AT: Added .ToList() call in order to prevent these validations being deferred 
            // thus calling these validation rules twice.
            var validList = inventoryItems.Where(x => IsActive(x)).ToList();

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
        /// Check whether InventoryItem is not a Bundle Product
        /// </summary>
        /// <param name="inventoryItem">InventoryItem that should be checked</param>
        /// <returns>Validation result</returns>
        private bool IsActive(InventoryItem inventoryItem)
        {
            InventoryItemPCExt itemPcExt = inventoryItem.GetExtension<InventoryItemPCExt>();

            bool isNotBundleProduct = itemPcExt == null || itemPcExt.UsrKNCompositeType == null || itemPcExt.UsrKNCompositeType != KCConstants.BundleProduct;

            return isNotBundleProduct;
        }
    }
}
