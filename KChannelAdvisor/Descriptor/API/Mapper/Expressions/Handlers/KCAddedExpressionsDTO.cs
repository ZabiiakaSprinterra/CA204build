namespace KChannelAdvisor.Descriptor.API.Mapper.Expressions.Handlers
{
    internal class KCAddedExpressionsDTO
    {
        public object LeftExpression { get; protected set; }
        public object RightExpression { get; protected set; }

        public KCAddedExpressionsDTO(object leftExpression, object rightExpression)
        {
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
        }

        public void SetLeft(object expression)
        {
            LeftExpression = expression;
        }

        public void SetRight(object expression)
        {
            RightExpression = expression;
        }
    }
}
