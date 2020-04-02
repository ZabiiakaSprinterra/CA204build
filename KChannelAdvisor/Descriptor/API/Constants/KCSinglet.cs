using System;
using System.Threading;

namespace KChannelAdvisor.Descriptor.API.Constants
{
    public static class KCSinglet
    {
        private static Lazy<KCJsonSerializer> _jsonSerializer;


        public static KCJsonSerializer JsonSerializer => _jsonSerializer.Value;



        static KCSinglet()
        {
            _jsonSerializer = new Lazy<KCJsonSerializer>(() => new KCJsonSerializer(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
