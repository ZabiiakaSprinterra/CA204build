using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.API.Request;
using KChannelAdvisor.ShippingService;
using System.ServiceModel;

namespace KChannelAdvisor.Descriptor.API.APIHelper
{
    public class KCShipmentAPIHelper
    {
        KCARestClient client;
        KCShipmentRequest request;

        public KCShipmentAPIHelper(KCARestClient client)
        {
            this.client = client;
            request = new KCShipmentRequest(client.ApiAccessToken);
        }

        public KCErrorResponse MarkTheOrderAsShipped(ODataShipment shipment, int? orderId)
        {
            return client.Post<KCErrorResponse>(request.AddShipment(shipment, orderId));
        }

        //17.07.19: KA: Request to SOAP Shipment service
        public APIResultOfArrayOfShippingCarrier GetShippingCarriers()
        {
            EndpointAddress endpoint = new EndpointAddress(client._xSiteMaster.EndpointAddressValueShipment);
            ShippingServiceSoapClient service = new ShippingServiceSoapClient(new BasicHttpsBinding(), endpoint);
            return service.GetShippingCarrierList(new APICredentials()
            {
                DeveloperKey = client._xSiteMaster.DevKey,
                Password = client._xSiteMaster.DevPassword
            }, client._xSiteMaster.AccountId);
        }
    }
}
