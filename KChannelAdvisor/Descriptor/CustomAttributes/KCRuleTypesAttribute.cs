using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using PX.Data;

namespace KChannelAdvisor.Descriptor.CustomAttributes
{
    public class KCRuleTypesAttribute : PXStringListAttribute
    {
        public KCRuleTypesAttribute() : base(
            new[]
            {
                Pair(KCRuleTypesConstants.Simple     ,KCRuleTypesConstants.KCUI.Simple ),
                Pair(KCRuleTypesConstants.Expression ,KCRuleTypesConstants.KCUI.Expression ),
            })
        { }
    }
}
