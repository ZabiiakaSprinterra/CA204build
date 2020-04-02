using PX.Common;

namespace KChannelAdvisor.Descriptor.CustomAttributes.Constants
{
    public class KCInventoryTrackingRulesConstants
    {
        public const string Consolidate = "Consolidate";
        public const string Manage = "Manage";

        [PXLocalizable]
        public class KCUI
        {
            public const string Consolidate = "Consolidate Inventory across all warehouses";
            public const string Manage = "Manage and Sync Inventory at Distribution Centers";
        }
    }
}
