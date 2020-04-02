using KChannelAdvisor.DAC;
using PX.Data;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.Descriptor.DataSlots
{
    public class KCAttributesMappingDatabaseSlot : IPrefetchable
    {
        protected List<KNSIKCClassificationsMapping> classMappings = new List<KNSIKCClassificationsMapping>();

        public static List<KNSIKCClassificationsMapping> ClassMappings => PXDatabase.GetSlot<KCAttributesMappingDatabaseSlot>(nameof(KCAttributesMappingDatabaseSlot), typeof(KNSIKCClassificationsMapping)).classMappings;

        public void Prefetch()
        {
            var iList = PXDatabase.SelectRecords<KNSIKCClassificationsMapping>().ToList();

            classMappings.AddRange(iList);
        }


    }
}
