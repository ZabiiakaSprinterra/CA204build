
namespace KChannelAdvisor.Descriptor.API.Mapper.Expressions.ExprHandlers
{
    internal class KCAddExprHandler : KCAbstractExprHandler
    {
        private readonly char _separator = KCExprEngine.Operators[nameof(KCConstants.Add)];

        public override KCIExpression Handle(string exprFormula)
        {
            var index = exprFormula.IndexOf(_separator);

            if (!string.IsNullOrWhiteSpace(exprFormula) && index > 0)
            {
                var leftExprFormula = exprFormula.Substring(0, index).Trim();
                var rightExprFormula = exprFormula.Substring(index + 1).Trim();
                return new KCAddExpression(KCExprEngine.HandlingChain.Handle(leftExprFormula), 
                                         KCExprEngine.HandlingChain.Handle(rightExprFormula));
            }
            else
            {
                return base.Handle(exprFormula);
            }
        }
    }
}
