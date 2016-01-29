using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DR.Common.RESTClient
{
    public class JsonClient : RESTClient, IJsonClient
    {

        public JsonClient() : this(false) { }
        public JsonClient(bool useISODates)
        {
            _useISODates = useISODates;
            ContentType = "application/x-www-form-urlencoded";
            Accept = "application/json, text/javascript, */*; q=0.01";
            _baseUrl = "";
        }

        private bool _useISODates { get; set; }
        
        public override T DeserializeObject<T>(string s)
        {
            try
            {
                return _useISODates
                    ? JsonConvert.DeserializeObject<T>(s, new IsoDateTimeConverter())
                    : JsonConvert.DeserializeObject<T>(s);
            }
            catch (JsonException e)
            {
                throw new RESTJsonException(s, e.Message, e);
            }
        }

        protected override string SerializeObject(object o)
        {
            return _useISODates ? JsonConvert.SerializeObject(o, new IsoDateTimeConverter()) : JsonConvert.SerializeObject(o);
        }
    }
}