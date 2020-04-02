namespace KChannelAdvisor.Descriptor.API.Mapper.Expressions.ExprHandlers
{
    internal interface KCIExprHandler
    {
        KCIExprHandler SetNext(KCIExprHandler handler);

        KCIExpression Handle(string exprFormula);
    }
}
