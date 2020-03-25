using KChannelAdvisor.BLC;
using KChannelAdvisor.DAC;
using PX.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.Descriptor.CustomAttributes
{
    public class KCCRAttributeSelectorAttribute : PXCustomSelectorAttribute
    {
        public KCCRAttributeSelectorAttribute() : base(typeof(KCAttribute.attributeID), typeof(KCAttribute.attributeName))
        {
            SubstituteKey = typeof(KCAttribute.attributeName);
        }

        protected virtual IEnumerable GetRecords()
        {
            KCCrossReferenceMappingMaint graph = (KCCrossReferenceMappingMaint)_Graph;
            IEnumerable<KCAttributesMapping> attributesMapping = graph.AttributesMappings.Select().RowCast<KCAttributesMapping>();
            IEnumerable<KCAttribute> allAttributes = graph.KCAttributes.Select().RowCast<KCAttribute>();
            
            foreach(KCAttribute attr in allAttributes)
            {
                if (!attributesMapping.Any(x => x.CAAttributeID == attr.AttributeID))
                {
                    yield return attr;
                }
            }
        }
    }
}
