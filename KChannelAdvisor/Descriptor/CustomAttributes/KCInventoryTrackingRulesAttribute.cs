using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using PX.Data;

namespace KChannelAdvisor.Descriptor.CustomAttributes
{
    public class KCInventoryTrackingRulesAttribute : PXStringListAttribute
    {
        public KCInventoryTrackingRulesAttribute() : base(
            new[]
            {
                    Pair(KCInventoryTrackingRulesConstants.Consolidate, KCInventoryTrackingRulesConstants.KCUI.Consolidate),
                    Pair(KCInventoryTrackingRulesConstants.Manage, KCInventoryTrackingRulesConstants.KCUI.Manage)
            })
        { }
    }
}
