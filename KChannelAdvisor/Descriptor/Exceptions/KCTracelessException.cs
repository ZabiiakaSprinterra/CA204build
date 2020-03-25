using System;

namespace KChannelAdvisor.Descriptor.Exceptions
{
    public class KCTracelessException : Exception
    {
        public KCTracelessException(string message) : base(message)
        {

        }

        public KCTracelessException() : base()
        {

        }

        public override string StackTrace => string.Empty;
    }
}
