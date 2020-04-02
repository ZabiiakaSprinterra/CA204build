using Newtonsoft.Json;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using System.IO;

namespace KChannelAdvisor.Descriptor.API
{
    public class KCJsonSerializer : ISerializer, IDeserializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _serializer;


        public string DateFormat { get; set; }

        public string RootElement { get; set; }

        public string Namespace { get; set; }

        public string ContentType { get; set; }



        public KCJsonSerializer()
        {
            ContentType = "application/json";
            _serializer = new Newtonsoft.Json.JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
            };
        }

        public KCJsonSerializer(Newtonsoft.Json.JsonSerializer serializer)
        {
            ContentType = "application/json";
            _serializer = serializer;
        }



        public string Serialize(object obj)
        {
            var result = "";
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Formatting.None;
                    jsonTextWriter.QuoteChar = '"';

                    _serializer.Serialize(jsonTextWriter, obj);

                    jsonTextWriter.Flush();
                    stringWriter.Flush();
                    result = stringWriter.ToString();
                }
            }

            return result;
        }

        public T Deserialize<T>(RestSharp.IRestResponse response)
        {
            T result;

            using (var stringReader = new StringReader(response.Content))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    result = _serializer.Deserialize<T>(jsonTextReader);
                }
            }

            return result;
        }

    }
}
