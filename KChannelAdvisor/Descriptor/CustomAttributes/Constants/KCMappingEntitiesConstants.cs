using PX.Common;
using PX.Data;

namespace KChannelAdvisor.Descriptor.CustomAttributes.Constants
{
    public class KCMappingEntitiesConstants
    {
        public const string Order   = "Order Import";
        public const string Product = "Product Export";


        [PXLocalizable]
        public class KCUI
        {
            public const string Order   = "Order Import";
            public const string Product = "Product Export";
        }

        #region BQL Constants        
        public class order : Constant<string>
        {
            public order() : base(Order)
            {
            }
        }
        public class product : Constant<string>
        {
            public product() : base(Product)
            {
            }
        }
        #endregion
    }
}
