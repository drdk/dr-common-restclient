using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DR.Common.RESTClient
{
    public class JsonClient : RESTClient, IJsonClient
    {

        public JsonClient() : this(false) { }
        public JsonClient(bool useISODates)
        {
            
            ContentType = "application/x-www-form-urlencoded";
            Accept = "application/json, text/javascript, */*; q=0.01";
            _baseUrl = "";
            var jsonConverter = new List<JsonConverter>();

            if (useISODates)
                jsonConverter.Add(new IsoDateTimeConverter());

            _jsonConverters = jsonConverter.ToArray();

        }

        private readonly JsonConverter[] _jsonConverters;
        
        public override T DeserializeObject<T>(string s)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(s, _jsonConverters);
            }
            catch (JsonException e)
            {
                throw new RESTJsonException(s, e.Message, e);
            }
        }

        protected override string SerializeObject(object o)
        {
            return JsonConvert.SerializeObject(o, _jsonConverters);
        }
    }
}