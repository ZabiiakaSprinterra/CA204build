using Newtonsoft.Json;

namespace KChannelAdvisor.Descriptor.API.Entity
{
    public class ResponseError<T>
    {
        [JsonProperty("error")]
        public KCAPIError Error { get; set; }
    }

    public class KCAPIError
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
