
using KChannelAdvisor.Descriptor.Exceptions;

namespace KChannelAdvisor.Descriptor.API.Mapper.Expressions.Handlers
{
    internal abstract class KCAbstractHandler : KCIHandler
    {
        private KCIHandler _nextHandler;

        public KCIHandler SetNext(KCIHandler handler)
        {
            _nextHandler = handler;

            return handler;
        }

        public virtual object Handle(object request)
        {
            if (_nextHandler != null)
            {
                return _nextHandler.Handle(request);
            }
            else
            {
                return ThrowException();
            }
        }

        protected virtual object ThrowException()
        {
            throw new KCInvalidMappingExpressionException(KCMessages.UnknownOperationException);
        }
    }
}
