
using KChannelAdvisor.DAC;
using PX.Data;
using PX.Objects.CS;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.Descriptor.DataSlots
{
    public class KCAttributeDatabaseSlot : IPrefetchable
    {
        protected List<KCAttribute> attr = new List<KCAttribute>();
        protected List<KCAttributesMapping> attrmap = new List<KCAttributesMapping>();
        protected List<CSAnswers> answ = new List<CSAnswers>();
        protected static PXSelectJoin<KCAttribute,
              LeftJoin<KCAttributesMapping, On<KCAttribute.attributeID, Equal<KCAttributesMapping.cAAttributeID>>,
              LeftJoin<CSAnswers, On<KCAttributesMapping.aAttributeName, Equal<CSAnswers.attributeID>>>>,
                 Where<KCAttribute.attributeName, Equal<Required<KCAttribute.attributeName>>,
                   And<Where<KCAttributesMapping.isMapped, Equal<True>,
                   And<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>>>>>> View;

        public static List<KCAttribute> ItemRef => PXDatabase.GetSlot<KCAttributeDatabaseSlot>(nameof(KCAttributeDatabaseSlot), typeof(KCAttribute)).attr;


        public void Prefetch()
        {
            var iList = PXDatabase.SelectRecords<KCAttribute>().ToList();

            attr.AddRange(iList);
        }

        public static PXSelectJoin<KCAttribute,
              LeftJoin<KCAttributesMapping, On<KCAttribute.attributeID, Equal<KCAttributesMapping.cAAttributeID>>,
              LeftJoin<CSAnswers, On<KCAttributesMapping.aAttributeName, Equal<CSAnswers.attributeID>>>>,
                 Where<KCAttribute.attributeName, Equal<Required<KCAttribute.attributeName>>,
                   And<Where<KCAttributesMapping.isMapped, Equal<True>,
                   And<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>>>>>>
        GetView()
        {

            return View;
        }
    }
}