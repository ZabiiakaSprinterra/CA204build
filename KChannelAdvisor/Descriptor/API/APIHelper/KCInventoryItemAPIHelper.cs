
using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.Descriptor.API.LoggerProvider;
using KChannelAdvisor.API.Request;
using KChannelAdvisor.InventoryService;
using KChannelAdvisor.Descriptor.Logger;
using RestSharp;
using System.Collections.Generic;
using System.ServiceModel;

namespace KChannelAdvisor.Descriptor.API.APIHelper
{
    public class KCInventoryItemAPIHelper
    {
        private KCARestClient client;
        private KCProductRequest request;
        private KCILoggerProvider logger;

        public KCInventoryItemAPIHelper(KCARestClient client)
        {
            this.client = client;
            request = new KCProductRequest(client.ApiAccessToken);
        }

        public KCInventoryItemAPIHelper(KCARestClient client, KCLoggerProperties loggerProperties)
        {
            this.client = client;
            request = new KCProductRequest(client.ApiAccessToken);
            logger = new KCLoggerProvider(loggerProperties);
        }

        public List<KCAPIInventoryItem> GetProducts()
        {
            List<KCAPIInventoryItem> items = new List<KCAPIInventoryItem>();
            KCODataWrapper<KCAPIInventoryItem> oDataItems = new KCODataWrapper<KCAPIInventoryItem>();

            do
            {
                IRestRequest dcRequest = request.GetProductsRequest(oDataItems.NextLink?.Replace(KCARestClient.BASE_URL, ""));
                oDataItems = client.Get<KCODataWrapper<KCAPIInventoryItem>>(dcRequest);
                items.AddRange(oDataItems.Value);
            }
            while (!string.IsNullOrEmpty(oDataItems.NextLink));

            return items;
        }

        public KCODataWrapper<KCAPIBundleComponent> GetBundleComponents(int? productId)
        {
            return client.Get<KCODataWrapper<KCAPIBundleComponent>>(request.GetBundleComponentsRequest(productId));
        }

        public KCODataWrapper<KCAPIInventoryItemIDs> GetProductIDsBySku(string inventoryItemCd)
        {
            return client.Get<KCODataWrapper<KCAPIInventoryItemIDs>>(request.GetProductIDsBySkuRequest(inventoryItemCd.Trim()));
        }

        public void DeleteBundleComponent(int? productId, int? componentId, string parentCd, string childCd)
        {
            logger.SetParentAndEntityIds(parentCd, childCd);
            client.Delete<KCAPIResponse>(request.DeleteBundleComponentRequest(productId, componentId));
        }

        public void UpdateQuantity(APIQuantityValue qty, int? productId)
        {
            client.Post<KCAPIResponse>(request.UpdateQuantityRequest(qty, productId));
        }

        public void EditProduct(KCAPIInventoryItem apiProduct, int? productId, string parentCd = null)
        {
            client.Patch<KCAPIResponse>(request.EditProductRequest(apiProduct, productId));
        }

        public List<KCAPIIdSkuJuxtaposion> GetAllIdSkuJuxtaposions()
        {
            List<KCAPIIdSkuJuxtaposion> juxs = new List<KCAPIIdSkuJuxtaposion>();
            KCODataWrapper<KCAPIIdSkuJuxtaposion> oDataJuxs = new KCODataWrapper<KCAPIIdSkuJuxtaposion>();

            do
            {
                IRestRequest jRequest = request.GetIdSkuJuxtaposions(oDataJuxs.NextLink?.Replace(KCARestClient.BASE_URL, ""));
                oDataJuxs = client.Get<KCODataWrapper<KCAPIIdSkuJuxtaposion>>(jRequest);
                juxs.AddRange(oDataJuxs.Value);
            }
            while (!string.IsNullOrEmpty(oDataJuxs.NextLink));

            return juxs;
        }

        public IEnumerable<KCAPIProductLabel> GetProductLabels(int? productId)
        {
            KCODataWrapper<KCAPIProductLabel> result = client.Get<KCODataWrapper<KCAPIProductLabel>>(request.GetProductLabelsRequest(productId));
            return result?.Value ?? new List<KCAPIProductLabel>();
        }

        public KCODataWrapper<KCAPIAttribute> GetAttributes(int? productId)
        {
            return client.Get<KCODataWrapper<KCAPIAttribute>>(request.GetAttributesRequest(productId));
        }

        //28.02.19: KA: Request to SOAP Inventory service
        public APIResultOfArrayOfClassificationConfigurationInformation GetClassifications()
        {
            EndpointAddress endpoint = new EndpointAddress(client._xSiteMaster.EndpointAddressValueInventory);
            InventoryServiceSoapClient service = new InventoryServiceSoapClient(new BasicHttpsBinding(), endpoint);

            return service.GetClassificationConfigurationInformation(new APICredentials()
            {
                DeveloperKey = client._xSiteMaster.DevKey,
                Password = client._xSiteMaster.DevPassword
            }, client._xSiteMaster.AccountId);
        }

        public List<KCAPIDistributionCenter> GetDistributionCenters()
        {
            List<KCAPIDistributionCenter> distributionCenters = new List<KCAPIDistributionCenter>();
            KCODataWrapper<KCAPIDistributionCenter> oDataDistributionCenter = new KCODataWrapper<KCAPIDistributionCenter>();

            do
            {
                IRestRequest dcRequest = request.GetDistributionCenters(oDataDistributionCenter.NextLink?.Replace(KCARestClient.BASE_URL, ""));
                oDataDistributionCenter = client.Get<KCODataWrapper<KCAPIDistributionCenter>>(dcRequest);
                if(oDataDistributionCenter.Value != null) distributionCenters.AddRange(oDataDistributionCenter.Value);
            }
            while (!string.IsNullOrEmpty(oDataDistributionCenter.NextLink));

            return distributionCenters;
        }

        public List<KCAPIProfile> GetProfiles()
        {
            List<KCAPIProfile> profiles = new List<KCAPIProfile>();
            KCODataWrapper<KCAPIProfile> oDataWrapper = new KCODataWrapper<KCAPIProfile>();

            do
            {
                IRestRequest dcRequest = request.GetProfiles(oDataWrapper.NextLink?.Replace(KCARestClient.BASE_URL, ""));
                oDataWrapper = client.Get<KCODataWrapper<KCAPIProfile>>(dcRequest);
                profiles.AddRange(oDataWrapper.Value);
            }
            while (!string.IsNullOrEmpty(oDataWrapper.NextLink));

            return profiles;
        }
    }
}
