using PX.Data;

namespace KChannelAdvisor.DAC.Helper
{
    public class KCEntities
    {
        public const string Select = "<SELECT>";
        public const string Order = "Orders Import";
        public const string Shipment = "Shipments Export";
        public const string PNUpdates = "PN Updates";
        public const string RetrieveIds = "Retrieve Product IDs in ChannelAdvisor";

        public class KCEntitiesListAttribute : PXStringListAttribute
        {
            public KCEntitiesListAttribute() : base(
                new[]
                {
                    Pair(Select, Select),
                    Pair(Order, Order),
                    Pair(Shipment, Shipment),
                    Pair(RetrieveIds, RetrieveIds)
                })
            { }
        }
    }
}
