
using KChannelAdvisor.DAC;
using PX.Data;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.Descriptor.DataSlots
{
    public class CrossReferenceMappingDatabaseSlot : IPrefetchable
    {
        protected List<KCCrossReferenceMapping> cross = new List<KCCrossReferenceMapping>();
        protected static PXSelect<KCCrossReferenceMapping> View;
        public static List<KCCrossReferenceMapping> Cross => PXDatabase.GetSlot<CrossReferenceMappingDatabaseSlot>(nameof(CrossReferenceMappingDatabaseSlot), typeof(KCCrossReferenceMapping)).cross;

        public void Prefetch()
        {
            var iList = PXDatabase.SelectRecords<KCCrossReferenceMapping>().ToList();

            cross.AddRange(iList);
        }

        public static PXSelect<KCCrossReferenceMapping> GetView()
        {

            return View;
        }
    }
}
