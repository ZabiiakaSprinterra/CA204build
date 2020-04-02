using KChannelAdvisor.BLC;
using KChannelAdvisor.DAC;
using ProductConfigurator.DAC;
using PX.Data;
using PX.Objects.IN;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.Descriptor.BulkUploader.ValidationChain
{
    internal class KCClassificationRule : KCAbstractRule
    {
        private IEnumerable<int?> ValidItemClasses { get; set; }
        private KCBulkProductMaint Graph { get; set; }

        public KCClassificationRule(KCBulkProductMaint graph)
        {
            Graph = graph;
        }

        public override IEnumerable<InventoryItem> Validate(IEnumerable<InventoryItem> inventoryItems)
        {
            ValidItemClasses = GetValidClassifications().Select(y => y.ItemClassID);
            // 06/05/19 AT: Added .ToList() call in order to prevent these validations being deferred 
            // thus calling these validation rules twice.
            var validList = inventoryItems.Where(x => ValidItemClasses.Contains(x.ItemClassID) && IsGroupChildrenValid(x)).ToList();

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
        /// Get List of Classifications, that were mapped to any ItemClass, and flag IsMapped for the Classification was set to true
        /// </summary>
        /// <returns>Valid Classifications</returns>
        private IEnumerable<DAC.KNSIKCClassificationsMapping> GetValidClassifications()
        {
            IEnumerable<DAC.KNSIKCClassificationsMapping> validClassifications = Graph.MappedClassifications.Select().RowCast<DAC.KNSIKCClassificationsMapping>();
            return validClassifications;
        }

        /// <summary>
        /// Check whether all Grouped item's children are having item classes, which are mapped to the any Classification
        /// </summary>
        /// <param name="itemClasses"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool IsGroupChildrenValid(InventoryItem product)
        {
            IEnumerable<KNSIGroupedItems> childItems = Graph.GroupedItemChilds.Select(product.InventoryID).RowCast<KNSIGroupedItems>();

            return !childItems.Any() || childItems.All(x => ValidItemClasses.Contains(x.ItemClass));
        }
    }
}
