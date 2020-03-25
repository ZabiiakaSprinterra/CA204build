using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.Descriptor.API.Base;
using KChannelAdvisor.Descriptor;
using RestSharp;

namespace KChannelAdvisor.API.Request
{
    public class KCProductRequest : KCRequestBase
    {
        public KCProductRequest(string token)
            : base(token)
        {
        }

        public IRestRequest GetProductsRequest(string parameter = null)
        {
            return CreateRestRequest(parameter ?? "/v1/Products");
        }

        public IRestRequest GetProductIDsBySkuRequest(string inventoryItemCD)
        {
            IRestRequest request = CreateRestRequest("/v1/Products?$filter=Sku eq '{sku}'&$select=ID,ParentProductID");
            request.AddUrlSegment("sku", inventoryItemCD);

            return request;
        }

        public IRestRequest GetProductLabelsRequest(int? productId)
        {
            IRestRequest request = CreateRestRequest("/v1/Products({id})/Labels");
            request.AddUrlSegment("id", productId.ToString());

            return request;
        }

        public IRestRequest GetBundleComponentsRequest(int? productId)
        {
            IRestRequest request = CreateRestRequest("/v1/Products({productId})/BundleComponents");
            request.AddUrlSegment("productId", productId.ToString());

            return request;
        }

        public IRestRequest GetAttributesRequest(int? productId)
        {
            IRestRequest request = CreateRestRequest("/v1/Products({productId})/Attributes");
            request.AddUrlSegment("productId", productId.ToString());

            return request;
        }

        public IRestRequest GetDistributionCenters(string link = null)
        {
            // 04/01/19 AT
            // This request uses OData filtering options
            // In order to obtain non-deleted Distribution Centers,
            // which has Type "Warehouse", "DropShip" or "RetailStore"      
            string filter = $"IsDeleted eq false and (Type eq '{KCConstants.Warehouse}' or Type eq '{KCConstants.DropShip}' or Type eq '{KCConstants.RetailStore}')";
            IRestRequest request = CreateRestRequest(link ?? $"/v1/DistributionCenters?$filter={filter}");
            return request;
        }

        public IRestRequest GetProfiles(string link = null)
        {
            IRestRequest request = CreateRestRequest(link ?? $"/v1/Profiles");
            return request;
        }

        public IRestRequest GetIdSkuJuxtaposions(string link = null)
        {
            string select = "$select=ID, Sku";
            IRestRequest request = CreateRestRequest(link ?? $"v1/Products?{select}");
            return request;
        }

        public IRestRequest UpdateQuantityRequest(APIQuantityValue quantity, int? productId)
        {
            IRestRequest request = CreateRestRequest("/v1/Products({id})/UpdateQuantity");
            request.AddUrlSegment("id", productId.ToString());
            AddBody(request, quantity);

            return request;
        }

        public IRestRequest EditProductRequest(KCAPIInventoryItem product, int? productId)
        {
            IRestRequest request = CreateRestRequest("/v1/Products({id})");
            request.AddUrlSegment("id", productId.ToString());
            AddBody(request, product);

            return request;
        }

        public IRestRequest DeleteBundleComponentRequest(int? productId, int? bundleComponentId)
        {
            var request = CreateRestRequest("/v1/Products({productId})/BundleComponents({componentId})");
            request.AddUrlSegment("productId", productId.ToString());
            request.AddUrlSegment("componentId", bundleComponentId.ToString());

            return request;
        }
    }
}
