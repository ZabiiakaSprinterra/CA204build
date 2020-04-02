using System;
using PX.Data;
using PX.Data.BQL;
using KChannelAdvisor.Descriptor.CustomAttributes;

namespace KChannelAdvisor.DAC
{
    [PXProjection(typeof(Select<KCSiteMaster>))]
    public class KCInventoryTrackingRule : IBqlTable
    {
        #region InventoryTrackingRule
        [KCInventoryTrackingRules]
        [PXDBString(BqlField = typeof(KCSiteMaster.inventoryTrackingRule))]
        [PXUIField(DisplayName = "Inventory Tracking Rule")]
        [PXDefault(typeof(Search<KCSiteMaster.inventoryTrackingRule>))]
        public virtual string InventoryTrackingRule { get; set; }
        public abstract class inventoryTrackingRule : BqlString.Field<inventoryTrackingRule> { }
        #endregion

        #region IncludeVendorInventory
        [PXDBBool(BqlField = typeof(KCSiteMaster.includeVendorInventory))]
        [PXDefault(typeof(Search<KCSiteMaster.includeVendorInventory>))]
        [PXUIField(DisplayName = "Include Vendor Inventory")]
        public virtual bool? IncludeVendorInventory { get; set; }
        public abstract class includeVendorInventory : BqlBool.Field<includeVendorInventory> { }
        #endregion

        #region DefaultDistributionCenterID
        [PXDBInt(BqlField = typeof(KCSiteMaster.defaultDistributionCenterID))]
        [PXDefault(typeof(Search<KCSiteMaster.defaultDistributionCenterID>))]
        [PXUIField(DisplayName = "Default ChannelAdvisor Distribution Center")]
        [PXSelector(typeof(Search<KCDistributionCenter.distributionCenterID>), 
            new Type[] { typeof(KCDistributionCenter.distributionCenterName) }, 
            SubstituteKey = typeof(KCDistributionCenter.distributionCenterName))]
        public virtual int? DefaultDistributionCenterID { get; set; }
        public abstract class defaultDistributionCenterID : BqlInt.Field<defaultDistributionCenterID> { }
        #endregion
    }
}
