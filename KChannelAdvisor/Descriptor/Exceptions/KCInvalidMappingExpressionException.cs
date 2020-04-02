using System;

namespace KChannelAdvisor.Descriptor.Exceptions
{
    [Serializable]
    public class KCInvalidMappingExpressionException : Exception
    {
        public const string InvalidMappingExpressionPrefix = "The specified mapping expression is invalid: ";

        public KCInvalidMappingExpressionException() { }

        public KCInvalidMappingExpressionException(string message) : base(message) { }

        public KCInvalidMappingExpressionException(string message, Exception inner) : base(InvalidMappingExpressionPrefix + message, inner) { }
    }
}
