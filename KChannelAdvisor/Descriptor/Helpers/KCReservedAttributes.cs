using KChannelAdvisor.DAC;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.Helpers
{
    public static class KCReservedAttributes
    {
        private static List<KCAttribute> reservedAttributes;

        public static IEnumerable<KCAttribute> GetAttributes()
        {
            return GetReservedAttributes();
        }

        private static IEnumerable<KCAttribute> GetReservedAttributes()
        {
            string reservedType = "Reserved";

            reservedAttributes = new List<KCAttribute>
            {
                new KCAttribute { AttributeName = "ASIN",                     AttributeType = reservedType },
                new KCAttribute { AttributeName = "Brand",                    AttributeType = reservedType },
                new KCAttribute { AttributeName = "Condition",                AttributeType = reservedType },
                new KCAttribute { AttributeName = "HarmonizedCode",           AttributeType = reservedType },
                new KCAttribute { AttributeName = "Height",                   AttributeType = reservedType },
                new KCAttribute { AttributeName = "ISBN",                     AttributeType = reservedType },
                new KCAttribute { AttributeName = "Length",                   AttributeType = reservedType },
                new KCAttribute { AttributeName = "Manufacturer",             AttributeType = reservedType },
                new KCAttribute { AttributeName = "MPN",                      AttributeType = reservedType },
                new KCAttribute { AttributeName = "ShortDescription",         AttributeType = reservedType },
                new KCAttribute { AttributeName = "SupplierPO",               AttributeType = reservedType },
                new KCAttribute { AttributeName = "TaxProductCode",           AttributeType = reservedType },
                new KCAttribute { AttributeName = "UPC",                      AttributeType = reservedType },
                new KCAttribute { AttributeName = "Warranty",                 AttributeType = reservedType },
                new KCAttribute { AttributeName = "Width",                    AttributeType = reservedType }
            };

            return reservedAttributes;
        }
    }
}