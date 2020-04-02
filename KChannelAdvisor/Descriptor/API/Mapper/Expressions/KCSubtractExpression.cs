using KChannelAdvisor.Descriptor.API.Mapper.Expressions.Handlers;

namespace KChannelAdvisor.Descriptor.API.Mapper.Expressions
{
    internal class KCSubtractExpression : KCIExpression
    {
        private KCIExpression leftExpression;
        private KCIExpression rightExpression;

        public KCSubtractExpression(KCIExpression left, KCIExpression right)
        {
            leftExpression = left;
            rightExpression = right;
        }

        public object Interpret(KCMappingContext context)
        {
            KCAddedExpressionsDTO dto = new KCAddedExpressionsDTO(leftExpression.Interpret(context), rightExpression.Interpret(context));
            return SubtractHandlers.Handle(dto);
        }

        private KCIHandler SubtractHandlers
        {
            get
            {
                IntSubtractHandler intHandler = new IntSubtractHandler();
                DoubleSubtractHandler doubleHandler = new DoubleSubtractHandler();
                DecimalSubtractHandler decimalHandler = new DecimalSubtractHandler();
                StringSubtractHandler stringHandler = new StringSubtractHandler();
                IntDoubleSubtractHandler intDoubleHandler = new IntDoubleSubtractHandler();
                IntDecimalSubtractHandler intDecimalHandler = new IntDecimalSubtractHandler();
                DoubleIntSubtractHandler doubleIntHandler = new DoubleIntSubtractHandler();
                DoubleDecimalSubtractHandler doubleDecimalHandler = new DoubleDecimalSubtractHandler();
                DecimalIntSubtractHandler decimalIntHandler = new DecimalIntSubtractHandler();
                DecimalDoubleSubtractHandler decimalDoubleHandler = new DecimalDoubleSubtractHandler();
                UnhandledTypeSubtractHandler unhandledTypeSubtractHandler = new UnhandledTypeSubtractHandler();

                intHandler.SetNext(decimalHandler)
                    .SetNext(doubleHandler)
                    .SetNext(stringHandler)
                    .SetNext(intDoubleHandler)
                    .SetNext(intDecimalHandler)
                    .SetNext(doubleIntHandler)
                    .SetNext(doubleDecimalHandler)
                    .SetNext(decimalIntHandler)
                    .SetNext(decimalDoubleHandler)
                    .SetNext(unhandledTypeSubtractHandler);

                return intHandler;
            }
        }
    }
}
