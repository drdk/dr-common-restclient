using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DR.Common.RESTClient
{
    public class JsonClient : RESTClient, IJsonClient
    {
        public JsonClient()
        {
            UseISODates = false;
            ContentType = "application/x-www-form-urlencoded";

            _baseUrl = "";
        }

        private bool UseISODates { get; set; }

        public override T DeserializeContent<T>(RESTClientException exception)
        {
            return DeserializeObject<T>(exception.Content);
        }

        protected override HttpWebResponse RequestData(string method, string url, NetworkCredential credential, WebHeaderCollection headers, bool useDefaultCredentials)
        {
            try
            {
                var req = PrepareHttpWebRequest(url, method, headers);

                req.UseDefaultCredentials = useDefaultCredentials;
                if (credential != null)
                {
                    req.Credentials = credential;
                    req.PreAuthenticate = true;
                }

                return req.GetResponse() as HttpWebResponse;
            }
            catch (Exception e)
            {
                throw new RESTClientException(url, e);
            }

        }

        protected override HttpWebResponse SendData(string method, string url, object o, WebHeaderCollection headers)
        {
            byte[] data;
            if (o != null && (o.GetType().BaseType == typeof(Stream) || o.GetType() == typeof(Stream)))
            {
                using (var memStream = new MemoryStream())
                {
                    ((Stream)o).CopyTo(memStream);
                    data = memStream.ToArray();
                }
            }
            else
            {
                var json = SerializeObject(o);
                data = (new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)).GetBytes(json);
            }

            try
            {
                var req = PrepareHttpWebRequest(url, method, headers);

                req.ContentLength = data.Length;
                req.ContentType = ContentType;
                req.Accept = "application/json, text/javascript, */*; q=0.01";

                var s = req.GetRequestStream();
                s.Write(data, 0, data.Length);
                s.Close();

                return req.GetResponse() as HttpWebResponse;
            }
            catch (Exception e)
            {
                throw new RESTClientException(url, e);
            }
        }

        protected override T DeserializeObject<T>(string s)
        {
            try
            {
                return UseISODates
                    ? JsonConvert.DeserializeObject<T>(s, new IsoDateTimeConverter())
                    : JsonConvert.DeserializeObject<T>(s);
            }
            catch (JsonException e)
            {
                throw new RESTJsonException(s, e.Message, e);
            }
        }

        private string SerializeObject(object o)
        {
            return UseISODates ? JsonConvert.SerializeObject(o, new IsoDateTimeConverter()) : JsonConvert.SerializeObject(o);
        }
    }
}