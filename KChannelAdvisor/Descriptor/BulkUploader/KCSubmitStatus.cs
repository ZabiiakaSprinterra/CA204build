using Newtonsoft.Json;
using System;

namespace KChannelAdvisor.Descriptor.BulkUploader
{
    internal class KCSubmitStatus
    {
        [JsonProperty(PropertyName = "$id")]
        public string Id { get; set; }

        public string Token { get; set; }

        public string Status { get; set; }

        public DateTime StartedOnUtc { get; set; }

        public object ResponseFileUrl { get; set; }
    }
}
