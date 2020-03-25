using PX.Data;
using KChannelAdvisor.Descriptor.Helpers;

namespace KChannelAdvisor.Descriptor.CustomAttributes
{
    public class KCSearchTypesListAttribute : PXStringListAttribute
    {
        public KCSearchTypesListAttribute() : base(
            new[]
            {
                    Pair(KCSearchTypes.ContainsRule, KCSearchTypes.ContainsRule),
                    Pair(KCSearchTypes.EqualsRule, KCSearchTypes.EqualsRule),
                    Pair(KCSearchTypes.StartsWithRule, KCSearchTypes.StartsWithRule),
                    Pair(KCSearchTypes.EndsWithRule, KCSearchTypes.EndsWithRule)
            })
        { }
    }
}
