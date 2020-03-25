using KChannelAdvisor.BLC;
using KChannelAdvisor.DAC;
using PX.Data;
using System.Collections;
using System.Collections.Generic;
using KChannelAdvisor.Descriptor.DataSlots;
using System.Linq;

namespace KChannelAdvisor.Descriptor.CustomAttributes
{
    class KCProductAttributeSelectorAttribute : PXCustomSelectorAttribute
    {
        public KCProductAttributeSelectorAttribute() : base(typeof(KCAttribute.attributeID), typeof(KCAttribute.attributeName))
        {
            SubstituteKey = typeof(KCAttribute.attributeName);
        }

        protected virtual IEnumerable GetRecords()
        {
            KCAttributesMappingMaint graph = (KCAttributesMappingMaint)_Graph;
            IEnumerable<KCCrossReferenceMapping> crossReferenceMapping = CrossReferenceMappingDatabaseSlot.Cross;// graph.CrossReferenceMapping.Select().RowCast<KCCrossReferenceMapping>();
            IEnumerable<KCAttribute> allAttributes = graph.Attributes.Select().RowCast<KCAttribute>();

            foreach (KCAttribute attr in allAttributes)
            {
                if (!crossReferenceMapping.Any(x => x.CAAttributeID == attr.AttributeID))
                {
                    yield return attr;
                }
            }
        }
    }
}
