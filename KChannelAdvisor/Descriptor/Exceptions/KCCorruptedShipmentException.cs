using System;

namespace KChannelAdvisor.Descriptor.Exceptions
{
    public class KCCorruptedShipmentException : Exception
    {
        public KCCorruptedShipmentException(string message) : base(message)
        {

        }

        public KCCorruptedShipmentException() : base()
        {

        }

        public override string StackTrace => string.Empty;
    }
}
