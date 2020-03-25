using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using PX.Data;

namespace KChannelAdvisor.Descriptor.CustomAttributes
{
    public class KCMappingEntititesAttribute : PXStringListAttribute
    {
        public KCMappingEntititesAttribute() : base(
            new[]
            {
                Pair(KCMappingEntitiesConstants.Order  , KCMappingEntitiesConstants.KCUI.Order),
                Pair(KCMappingEntitiesConstants.Product, KCMappingEntitiesConstants.KCUI.Product),
            })
        { }
    }
}
