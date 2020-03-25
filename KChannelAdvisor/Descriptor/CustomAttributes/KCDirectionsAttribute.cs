using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using PX.Data;

namespace KChannelAdvisor.Descriptor.CustomAttributes
{
    public class KCDirectionsAttribute : PXStringListAttribute
    {
        public KCDirectionsAttribute() : base(
            new[]
            {
                Pair(KCDirectionsConstants.Import, KCDirectionsConstants.KCUI.Import),
                Pair(KCDirectionsConstants.Export, KCDirectionsConstants.KCUI.Export),
            })
        { }
    }
}
