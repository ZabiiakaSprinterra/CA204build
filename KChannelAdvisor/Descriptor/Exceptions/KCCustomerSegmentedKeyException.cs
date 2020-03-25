using System;

namespace KChannelAdvisor.Descriptor.Exceptions
{
    public class KCCustomerSegmentedKeyException : Exception
    {
        public const string error = "Segmented Key/Numbering sequence CUSTOMER doesn't exist in the system. Orders couldn't be imported.";

        public KCCustomerSegmentedKeyException() : base(error) { }

        public KCCustomerSegmentedKeyException(string err) : base(err) { }
    }
}
