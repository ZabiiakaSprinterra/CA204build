using KChannelAdvisor.BLC;
using KChannelAdvisor.DAC;
using PX.Data;
using System.Collections;
using System.Linq;

namespace KChannelAdvisor.Descriptor.CustomAttributes
{
    public class KCAViewSelectorAttribute : PXCustomSelectorAttribute
    {
        public KCAViewSelectorAttribute() : base(typeof(KCAcumaticaMappingField.viewDisplayName),
                                                  typeof(KCAcumaticaMappingField.viewDisplayName))
        {

        }

        public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e) { }

        protected virtual IEnumerable GetRecords()
        {
            KCMappingMaint graph = (KCMappingMaint)_Graph;
            var type = graph.MappingSetupFilter.Current?.MappingEntity;
            var view = graph.AcumaticaFields;
            return view.Select(type)
                       .RowCast<KCAcumaticaMappingField>()
                       .GroupBy(i => i.ViewName)
                       .Select(i => i.First())
                       .ToList();
        }
    }
}
