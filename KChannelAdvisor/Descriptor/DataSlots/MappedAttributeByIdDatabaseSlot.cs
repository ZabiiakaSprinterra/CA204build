using KChannelAdvisor.DAC;
using PX.Data;
using System.Collections.Generic;
using System.Linq;


namespace KChannelAdvisor.Descriptor.DataSlots
{
    public class MappedAttributeByIdDatabaseSlot : IPrefetchable
    {

        protected List<KCAttributesMapping> attrmap = new List<KCAttributesMapping>();
        protected static PXSelect<KCAttributesMapping> View;
        public static string _attributeID;
        public static List<KCAttributesMapping> Attrmap => PXDatabase.GetSlot<MappedAttributeByIdDatabaseSlot>(nameof(MappedAttributeByIdDatabaseSlot), typeof(KCAttributesMapping)).attrmap;

        public void Prefetch()
        {
            var iList = PXDatabase.SelectRecords<KCAttributesMapping>().ToList();

            attrmap.AddRange(iList);
        }

        public static PXSelect<KCAttributesMapping> GetView()
        {

            return View;
        }
    }

}
