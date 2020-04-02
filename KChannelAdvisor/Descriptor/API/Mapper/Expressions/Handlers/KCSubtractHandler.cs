
using KChannelAdvisor.Descriptor.Exceptions;

namespace KChannelAdvisor.Descriptor.API.Mapper.Expressions.Handlers
{
    internal abstract class KCSubtractHandler<T> : KCAbstractHandler
    {
        public override object Handle(object request)
        {
            if (request is KCAddedExpressionsDTO dto && dto.LeftExpression is T left && dto.RightExpression is T right)
            {
                return Subtract(left, right);
            }
            else
            {
                return base.Handle(request);
            }
        }

        protected abstract T Subtract(T left, T right);

        protected override object ThrowException()
        {
            throw new KCInvalidMappingExpressionException(KCMessages.SubtractHandlerException);
        }
    }

    internal abstract class SubtractHandler<T1, T2> : KCAbstractHandler
    {
        public override object Handle(object request)
        {
            if (request is KCAddedExpressionsDTO dto && dto.LeftExpression is T1 left && dto.RightExpression is T2 right)
            {
                return Subtract(left, right);
            }
            else
            {
                return base.Handle(request);
            }
        }

        protected abstract object Subtract(T1 left, T2 right);

        protected override object ThrowException()
        {
            throw new KCInvalidMappingExpressionException(KCMessages.SubtractHandlerException);
        }
    }

    internal class IntSubtractHandler : KCSubtractHandler<int>
    {
        protected override int Subtract(int left, int right) => left - right;
    }

    internal class DecimalSubtractHandler : KCSubtractHandler<decimal>
    {
        protected override decimal Subtract(decimal left, decimal right) => left - right;
    }

    internal class DoubleSubtractHandler : KCSubtractHandler<double>
    {
        protected override double Subtract(double left, double right) => left - right;
    }

    internal class StringSubtractHandler : KCSubtractHandler<string>
    {
        protected override string Subtract(string left, string right) => left.Trim() + " " + KCExprEngine.Operators[nameof(KCConstants.Subtract)] + " " + right.Trim();
    }

    internal class IntDoubleSubtractHandler : SubtractHandler<int, double>
    {
        protected override object Subtract(int left, double right) => left - right;
    }

    internal class IntDecimalSubtractHandler : SubtractHandler<int, decimal>
    {
        protected override object Subtract(int left, decimal right) => left - right;
    }

    internal class DoubleIntSubtractHandler : SubtractHandler<double, int>
    {
        protected override object Subtract(double left, int right) => left - right;
    }

    internal class DoubleDecimalSubtractHandler : SubtractHandler<double, decimal>
    {
        protected override object Subtract(double left, decimal right) => (decimal)left - right;
    }

    internal class DecimalDoubleSubtractHandler : SubtractHandler<decimal, double>
    {
        protected override object Subtract(decimal left, double right) => left - (decimal)right;
    }

    internal class DecimalIntSubtractHandler : SubtractHandler<decimal, int>
    {
        protected override object Subtract(decimal left, int right) => left - right;
    }

    internal class UnhandledTypeSubtractHandler : KCAbstractHandler
    {
        public override object Handle(object request)
        {
            if (request is KCAddedExpressionsDTO dto)
            {
                return dto.LeftExpression.ToString() + " " + KCExprEngine.Operators[nameof(KCConstants.Subtract)] + " " + dto.RightExpression.ToString();
            }
            else
            {
                return base.Handle(request);
            }
        }
    }
}
