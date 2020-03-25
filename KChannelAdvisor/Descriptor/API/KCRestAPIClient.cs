using RestSharp;
using RestSharp.Deserializers;

namespace KChannelAdvisor.Descriptor.API
{
    public class KCRestApiClient : RestClient
    {
        public KCRestApiClient(IDeserializer jsonDeserializer, string baseUrl)
            : base(baseUrl)
        {
            AddHandler("application/json", jsonDeserializer);
            AddHandler("text/json", jsonDeserializer);
            AddHandler("text/x-json", jsonDeserializer);
        }
    }
}
