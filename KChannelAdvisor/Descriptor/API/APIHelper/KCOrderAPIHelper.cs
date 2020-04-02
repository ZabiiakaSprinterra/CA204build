using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.API.Request;
using RestSharp;
using System;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.API.Helper
{
    public class KCOrderAPIHelper
    {
        KCARestClient client;
        KCOrderRequest request;



        public KCOrderAPIHelper(KCARestClient client)
        {
            this.client = client;
            request = new KCOrderRequest(client.ApiAccessToken);
        }



        public KCAPIOrder GetOrder(int? orderId)
        {
            return client.Get<KCAPIOrder>(request.GetOrder(orderId));
        }

        public List<KCAPIOrder> GetOrders(DateTime? dateFrom, DateTime? dateTo)
        {
            List<KCAPIOrder> items = new List<KCAPIOrder>();
            KCODataWrapper<KCAPIOrder> oDataItems = new KCODataWrapper<KCAPIOrder>();

            do
            {
                IRestRequest dcRequest = request.GetFilteredOrdersRequest(client._xSiteMaster.ProfileId, dateFrom, dateTo,
                                            oDataItems.NextLink?.Replace(KCARestClient.BASE_URL, ""));
                oDataItems = client.Get<KCODataWrapper<KCAPIOrder>>(dcRequest);
                items.AddRange(oDataItems.Value);
            }
            while (!string.IsNullOrEmpty(oDataItems.NextLink));

            return items;
        }

        public List<KCAPIOrderItem> GetOrderItems(int orderId)
        {
            var result = client.Get<KCODataWrapper<KCAPIOrderItem>>(request.GetOrderItems(orderId));
            return result?.Value;
        }

        public void SetOrderExported(int orderId)
        {
            client.Post<KCAPIOrder>(request.SetOrderExported(orderId));
        }

        public List<KCAPIFulfillment> GetFulfillments(int orderID)
        {
            List<KCAPIFulfillment> items = new List<KCAPIFulfillment>();
            KCODataWrapper<KCAPIFulfillment> oDataItems = new KCODataWrapper<KCAPIFulfillment>();

            do
            {
                IRestRequest dcRequest = request.GetFulfillments(orderID, oDataItems.NextLink?.Replace(KCARestClient.BASE_URL, ""));
                oDataItems = client.Get<KCODataWrapper<KCAPIFulfillment>>(dcRequest);
                items.AddRange(oDataItems.Value);
            }
            while (!string.IsNullOrEmpty(oDataItems.NextLink));

            return items;
        }
    }
}
