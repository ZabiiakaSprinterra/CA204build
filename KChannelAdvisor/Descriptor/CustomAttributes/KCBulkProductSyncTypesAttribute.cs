using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using PX.Data;

namespace KChannelAdvisor.Descriptor.CustomAttributes
{
    public class KCBulkProductSyncTypesAttribute : PXStringListAttribute
    {
        public KCBulkProductSyncTypesAttribute() : base(
            new[]
            {
                Pair(KCBulkProductSyncTypesConstants.Custom, KCBulkProductSyncTypesConstants.KCUI.Custom),
                Pair(KCBulkProductSyncTypesConstants.Delta, KCBulkProductSyncTypesConstants.KCUI.Delta),
                Pair(KCBulkProductSyncTypesConstants.Full, KCBulkProductSyncTypesConstants.KCUI.Full),
            })
        { }
    }
}
