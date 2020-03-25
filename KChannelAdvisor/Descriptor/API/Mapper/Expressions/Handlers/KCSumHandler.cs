
using KChannelAdvisor.Descriptor.Exceptions;

namespace KChannelAdvisor.Descriptor.API.Mapper.Expressions.Handlers
{
    internal abstract class KCSumHandler<T> : KCAbstractHandler
    {
        public override object Handle(object request)
        {
            if (request is KCAddedExpressionsDTO dto && dto.LeftExpression is T left && dto.RightExpression is T right)
            {
                return Sum(left, right);
            }
            else
            {
                return base.Handle(request);
            }
        }

        protected abstract T Sum(T left, T right);

        protected override object ThrowException()
        {
            throw new KCInvalidMappingExpressionException(KCMessages.SumHandlerException);
        }
    }

    internal abstract class KCSumHandler<T1, T2> : KCAbstractHandler
    {
        public override object Handle(object request)
        {
            if (request is KCAddedExpressionsDTO dto && dto.LeftExpression is T1 left && dto.RightExpression is T2 right)
            {
                return Sum(left, right);
            }
            else
            {
                return base.Handle(request);
            }
        }

        protected abstract object Sum(T1 left, T2 right);

        protected override object ThrowException()
        {
            throw new KCInvalidMappingExpressionException(KCMessages.SumHandlerException);
        }
    }

    internal class KCIntSumHandler : KCSumHandler<int>
    {
        protected override int Sum(int left, int right) => left + right;
    }

    internal class KCDecimalSumHandler : KCSumHandler<decimal>
    {
        protected override decimal Sum(decimal left, decimal right) => left + right;
    }

    internal class KCDoubleSumHandler : KCSumHandler<double>
    {
        protected override double Sum(double left, double right) => left + right;
    }

    internal class KCStringConcatHandler : KCSumHandler<string>
    {
        protected override string Sum(string left, string right) => left.Trim() + " " + right.Trim();
    }

    internal class KCIntDoubleSumHandler : KCSumHandler<int, double>
    {
        protected override object Sum(int left, double right) => left + right;
    }

    internal class KCIntDecimalSumHandler : KCSumHandler<int, decimal>
    {
        protected override object Sum(int left, decimal right) => left + right;
    }

    internal class KCDoubleIntSumHandler : KCSumHandler<double, int>
    {
        protected override object Sum(double left, int right) => left + right;
    }

    internal class KCDoubleDecimalSumHandler : KCSumHandler<double, decimal>
    {
        protected override object Sum(double left, decimal right) => (decimal)left + right;
    }

    internal class KCDecimalDoubleSumHandler : KCSumHandler<decimal, double>
    {
        protected override object Sum(decimal left, double right) => left + (decimal)right;
    }

    internal class KCDecimalIntSumHandler : KCSumHandler<decimal, int>
    {
        protected override object Sum(decimal left, int right) => left + right;
    }

    internal class KCUnhandledTypeConcatHandler : KCAbstractHandler
    {
        public override object Handle(object request)
        {
            if (request is KCAddedExpressionsDTO dto)
            {
                return dto.LeftExpression.ToString() + " " + dto.RightExpression.ToString();
            }
            else
            {
                return base.Handle(request);
            }
        }
    }
}
