using KChannelAdvisor.Descriptor.API.Constants;
using RestSharp;

namespace KChannelAdvisor.Descriptor.API.Base
{
    public class KCRequestBase
    {
        private string _token;
        private KCJsonSerializer _serializer;



        public KCRequestBase(string token)
        {
            _token = token;
            _serializer = KCSinglet.JsonSerializer;
        }



        protected IRestRequest CreateRestRequest(string resourceUrl)
        {
            var request = CreateRestRequest();
            request.Resource = resourceUrl;

            return request;
        }

        protected IRestRequest CreateRestRequest()
        {
            var request = new RestRequest();
            AddHeaders(request);

            return request;
        }

        protected void AddBody(IRestRequest request, object body)
        {
            request.JsonSerializer = _serializer;
            request.AddJsonBody(body);
        }


        private void AddHeaders(IRestRequest request)
        {
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + _token);
        }
    }
}
