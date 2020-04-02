using PX.Common;

namespace KChannelAdvisor.Descriptor.CustomAttributes.Constants
{
    public class KCBulkProductSyncTypesConstants
    {
        public const string Custom = "C";
        public const string Delta = "D";
        public const string Full = "F";

        [PXLocalizable]
        public class KCUI
        {
            public const string Custom = "Custom Sync";
            public const string Delta = "Delta Sync";
            public const string Full = "Full Sync";
        }
    }
}
