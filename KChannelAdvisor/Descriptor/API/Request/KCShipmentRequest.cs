using KChannelAdvisor.Descriptor.API.Base;
using RestSharp;

namespace KChannelAdvisor.API.Request
{
    public class KCShipmentRequest : KCRequestBase
    {
        public KCShipmentRequest(string token)
            : base(token)
        {
        }



        public IRestRequest AddShipment(object shipment, int? orderId)
        {
            var request = CreateRestRequest("/v1/Orders({id})/Ship");
            request.AddUrlSegment("id", orderId.ToString());
            AddBody(request, shipment);

            return request;
        }
    }
}
