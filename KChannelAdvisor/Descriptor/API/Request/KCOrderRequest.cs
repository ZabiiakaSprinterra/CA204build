
using KChannelAdvisor.Descriptor.API.Base;
using RestSharp;
using System;

namespace KChannelAdvisor.API.Request
{
    public class KCOrderRequest : KCRequestBase
    {
        public KCOrderRequest(string token)
            : base(token)
        {
        }

        public IRestRequest GetOrder(int? orderId)
        {
            var request = CreateRestRequest("/v1/Orders({id})");
            request.AddUrlSegment("id", orderId.ToString());
            return request;
        }

        public IRestRequest GetFilteredOrdersRequest(int? profileId, DateTime? dateFrom, DateTime? dateTo, string parameter = null)
        {
            string dateFromString = "";
            string dateToString = "";
            string pattern = "yyyy-MM-dd";
            bool dateFromEmpty = dateFrom.GetValueOrDefault() == default;
            bool dateToEmpty = dateTo.GetValueOrDefault() == default;
            if (!dateFromEmpty) dateFromString = dateFrom.GetValueOrDefault().ToString(pattern);
            if (!dateToEmpty) dateToString = dateTo.GetValueOrDefault().ToString(pattern);
            var link = "/v1/Orders?exported=false &$filter=" + (dateFromEmpty ? "" : ("(CreatedDateUtc ge " + dateFromString +
                                    ") and ")) + (dateToEmpty ? "" : ("(CreatedDateUtc le " + dateToString +
                                    ") and ")) + "(CheckoutStatus eq 'Completed' or  CheckoutStatus eq 'CompletedOffline') and " +
                                    "(PaymentStatus eq 'Submitted' or PaymentStatus eq 'Cleared' or PaymentStatus eq 'Deposited') and " +
                                    "(ProfileID eq {profileId})";
            var request = CreateRestRequest(parameter ?? link);
            request.AddUrlSegment("profileId", profileId.ToString());

            return request;
        }

        public IRestRequest GetFulfillments(int orderID, string parameter = null)
        {
            var link = "/v1/Fulfillments?$filter=OrderID eq " + orderID + "&$expand=Items";
            var request = CreateRestRequest(parameter ?? link);

            return request;
        }

        public IRestRequest GetOrderItems(int orderId)
        {
            var request = CreateRestRequest("/v1/Orders({id})/Items");
            request.AddUrlSegment("id", orderId.ToString());

            return request;
        }

        public IRestRequest SetOrderExported(int? orderId)
        {
            IRestRequest request = CreateRestRequest("/v1/Orders({id})/Export");
            request.AddUrlSegment("id", orderId.ToString());

            return request;
        }

    }
}
