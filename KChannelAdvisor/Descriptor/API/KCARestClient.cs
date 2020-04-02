using KChannelAdvisor.Descriptor.API.Constants;
using KChannelAdvisor.DAC;
using KChannelAdvisor.DAC.Helper;
using Newtonsoft.Json;
using PX.Data;
using RestSharp;

namespace KChannelAdvisor.Descriptor.API
{
    public class KCARestClient
    {
        public static  string BASE_URL;

        public KCSiteMaster _xSiteMaster;
        private KCRestApiClient _restClient;
        private Entity.KCAPIAccessToken _token;


        public string ApiAccessToken => _token?.Access_token;



        public KCARestClient(KCSiteMaster xSiteMaster)
        {
            this._xSiteMaster = xSiteMaster;
            BASE_URL = xSiteMaster?.BaseUrl;
            _restClient = new KCRestApiClient(KCSinglet.JsonSerializer, "https://api.channeladvisor.com");
            GetAccessToken();
        }



        public T Post<T>(IRestRequest request) where T : class, new()
        {
            request.Method = Method.POST;
            var resp = _restClient.Execute<T>(request);

            return resp.Data;
        }

        public T Get<T>(IRestRequest request) where T : class, new()
        {
            request.Method = Method.GET;
            var resp = _restClient.Execute<T>(request);

            return resp.Data;
        }

        public T Patch<T>(IRestRequest request) where T : class, new()
        {
            request.Method = Method.PATCH;
            var resp = _restClient.Execute<T>(request);

            return resp.Data;
        }

        public T Delete<T>(IRestRequest request) where T : class, new()
        {
            request.Method = Method.DELETE;
            var resp = _restClient.Execute<T>(request);

            return resp.Data;
        }


        private void GetAccessToken()
        {
            KCInternalReponse response = new KCSiteMasterHelper().VerifyApiAccess(_xSiteMaster);
            if (response.Data == null) throw new PXException(KCMessages.InternalServerError);
            _token = JsonConvert.DeserializeObject<Entity.KCAPIAccessToken>(response.Data);
        }
    }
}
