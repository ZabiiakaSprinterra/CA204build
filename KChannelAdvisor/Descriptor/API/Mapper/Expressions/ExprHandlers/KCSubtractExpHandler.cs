
namespace KChannelAdvisor.Descriptor.API.Mapper.Expressions.ExprHandlers
{
    internal class KCSubtractExpHandler : KCAbstractExprHandler
    {
        private readonly char _separator = KCExprEngine.Operators[nameof(KCConstants.Subtract)];

        public override KCIExpression Handle(string exprFormula)
        {
            int index = exprFormula.IndexOf(_separator);

            if (!string.IsNullOrWhiteSpace(exprFormula) && index > 0)
            {
                string leftExprFormula = exprFormula.Substring(0, index).Trim();
                string rightExprFormula = exprFormula.Substring(index + 1).Trim();
                return new KCSubtractExpression(KCExprEngine.HandlingChain.Handle(leftExprFormula),
                                              KCExprEngine.HandlingChain.Handle(rightExprFormula));
            }
            else
            {
                return base.Handle(exprFormula);
            }
        }
    }
}
