using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using PX.Data;

namespace KChannelAdvisor.Descriptor.CustomAttributes
{
    public class KCMappingRulesAttribute : PXStringListAttribute
    {
        public KCMappingRulesAttribute() : base(
            new[]
            {
                Pair(KCMappingRuleConstants.NA      ,KCMappingRuleConstants.KCUI.NA ),
                Pair(KCMappingRuleConstants.Static  ,KCMappingRuleConstants.KCUI.Static ),
                Pair(KCMappingRuleConstants.Mapping ,KCMappingRuleConstants.KCUI.Mapping ),
                Pair(KCMappingRuleConstants.Empty   ,KCMappingRuleConstants.KCUI.Empty ),
            })
        { }
    }
}
