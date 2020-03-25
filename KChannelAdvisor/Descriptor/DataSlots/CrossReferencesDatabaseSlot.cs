using PX.Data;
using PX.Objects.IN;
using System.Collections.Generic;
using System.Linq;


namespace KChannelAdvisor.Descriptor.DataSlots
{
    public class CrossReferencesDatabaseSlot : IPrefetchable
    {
        protected List<INItemXRef> itemRef = new List<INItemXRef>();
        protected static PXSelect<INItemXRef> View;
        public static List<INItemXRef> ItemRef => PXDatabase.GetSlot<CrossReferencesDatabaseSlot>(nameof(CrossReferencesDatabaseSlot), typeof(INItemXRef)).itemRef;

        public void Prefetch()
        {
            var iList = PXDatabase.SelectRecords<INItemXRef>().ToList();

            itemRef.AddRange(iList);
        }

        public static PXSelect<INItemXRef> GetView()
        {

            return View;
        }
    }
}
