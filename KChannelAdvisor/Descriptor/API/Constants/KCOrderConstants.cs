

namespace KChannelAdvisor.Descriptor.API.Constants
{
    public static class KCOrderConstants
    {
        public const string DeliveryStatusComplete = "Complete";
        public const string ExternallyManagedRollup = "ExternallyManaged";
        public const string SO = "SO";
        public const string SalesOrder = "Sales Order";
        public class SOConst : PX.Data.BQL.BqlString.Constant<SOConst>
        {
            public SOConst() : base(SO) { }
        }
        public class SalesOConst : PX.Data.BQL.BqlString.Constant<SOConst>
        {
            public SalesOConst() : base(SalesOrder) { }
        }
    }

}
