using KChannelAdvisor.Descriptor.API.Mapper.Expressions.Handlers;

namespace KChannelAdvisor.Descriptor.API.Mapper.Expressions
{
    internal class KCAddExpression : KCIExpression
    {
        private KCIExpression leftExpression;
        private KCIExpression rightExpression;

        public KCAddExpression(KCIExpression left, KCIExpression right)
        {
            leftExpression = left;
            rightExpression = right;
        }

        public object Interpret(KCMappingContext context)
        {
            KCAddedExpressionsDTO dto = new KCAddedExpressionsDTO(leftExpression.Interpret(context), rightExpression.Interpret(context));
            return SystemTypeSumHandlers.Handle(dto);
        }

        private KCIHandler SystemTypeSumHandlers
        {
            get
            {
                KCIntSumHandler intHandler = new KCIntSumHandler();
                KCDoubleSumHandler doubleHandler = new KCDoubleSumHandler();
                KCDecimalSumHandler decimalHandler = new KCDecimalSumHandler();
                KCStringConcatHandler stringHandler = new KCStringConcatHandler();
                KCIntDoubleSumHandler intDoubleHandler = new KCIntDoubleSumHandler();
                KCIntDecimalSumHandler intDecimalHandler = new KCIntDecimalSumHandler();
                KCDoubleIntSumHandler doubleIntHandler = new KCDoubleIntSumHandler();
                KCDoubleDecimalSumHandler doubleDecimalHandler = new KCDoubleDecimalSumHandler();
                KCDecimalIntSumHandler decimalIntHandler = new KCDecimalIntSumHandler();
                KCDecimalDoubleSumHandler decimalDoubleHandler = new KCDecimalDoubleSumHandler();
                KCUnhandledTypeConcatHandler unhandledTypeConcatHandler = new KCUnhandledTypeConcatHandler();

                intHandler.SetNext(decimalHandler)
                    .SetNext(doubleHandler)
                    .SetNext(stringHandler)
                    .SetNext(intDoubleHandler)
                    .SetNext(intDecimalHandler)
                    .SetNext(doubleIntHandler)
                    .SetNext(doubleDecimalHandler)
                    .SetNext(decimalIntHandler)
                    .SetNext(decimalDoubleHandler)
                    .SetNext(unhandledTypeConcatHandler);

                return intHandler;
            }
        }
    }
}
