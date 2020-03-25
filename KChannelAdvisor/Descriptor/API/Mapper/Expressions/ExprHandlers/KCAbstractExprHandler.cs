
using KChannelAdvisor.Descriptor.Exceptions;

namespace KChannelAdvisor.Descriptor.API.Mapper.Expressions.ExprHandlers
{
    internal abstract class KCAbstractExprHandler : KCIExprHandler
    {
        private KCIExprHandler _nextHandler;

        public KCIExprHandler SetNext(KCIExprHandler handler)
        {
            _nextHandler = handler;

            return _nextHandler;
        }

        public virtual KCIExpression Handle(string exprFormula)
        {
            if (_nextHandler != null)
            {
                return _nextHandler.Handle(exprFormula);
            }
            else
            {
                throw new KCInvalidMappingExpressionException();
            }
        }

        protected virtual object ThrowException()
        {
            throw new KCInvalidMappingExpressionException(KCMessages.TargetExpressionException);
        }
    }
}
