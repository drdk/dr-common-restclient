using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DR.Common.RESTClient
{
    public class RESTClient : IRESTClient
    {
        public RESTClient()
        {
            UseISODates = false;
            _baseUrl = "";
        }

        private string _baseUrl;
        public string BaseURL
        {
            get { return _baseUrl; }
            set
            {
                Uri temp;
                if (string.IsNullOrEmpty(value) || !Uri.TryCreate(value, UriKind.Absolute, out temp))
                {
                    throw new ArgumentException("Invalid service uri", "value");
                }
                _baseUrl = temp.ToString();
            }
        }
        public bool UseISODates { get; set; }

        public string Request(string method, string url, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false)
        {
            url = BaseURL.TrimEnd('/') + url;
            using (var resp = RequestData(method, url, credential, headers, useDefaultCredentials))
            {
                return ReadResponse(resp);
            }
        }

        public T Request<T>(string method, string url, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false) where T : class
        {
            return DeserializeObject<T>(Request(method, url, credential, headers, useDefaultCredentials));
        }

        public string Request(string method, string url, object o, WebHeaderCollection headers = null)
        {
            url = BaseURL.TrimEnd('/') + url;
            using (var resp = SendData(method, url, o, headers))
            {
                return ReadResponse(resp);
            }
        }

        public T Request<T>(string method, string url, object o, WebHeaderCollection headers = null) where T : class
        {
            return DeserializeObject<T>(Request(method, url, o, headers));
        }

        public string Get(string url, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false) { return Request("GET", url, credential, headers, useDefaultCredentials); }
        public T Get<T>(string url, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false) where T : class { return Request<T>("GET", url, credential, headers, useDefaultCredentials); }

        public string Delete(string url, WebHeaderCollection headers = null) { return Request("DELETE", url, headers); }
        public T Delete<T>(string url, WebHeaderCollection headers = null) where T : class { return Request<T>("DELETE", url, headers); }

        public string Post(string url, object o, WebHeaderCollection headers = null) { return Request("POST", url, o, headers); }
        public T Post<T>(string url, object o, WebHeaderCollection headers = null) where T : class { return Request<T>("POST", url, o, headers); }

        public string Put(string url, object o, WebHeaderCollection headers = null) { return Request("PUT", url, o, headers); }
        public T Put<T>(string url, object o, WebHeaderCollection headers = null) where T : class { return Request<T>("PUT", url, o, headers); }

        private HttpWebResponse RequestData(string method, string url, NetworkCredential credential, WebHeaderCollection headers, bool useDefaultCredentials)
        {
            try
            {
                var req = HttpWebRequest.Create(url) as HttpWebRequest;

                req.UseDefaultCredentials = useDefaultCredentials;
                if (credential != null) req.Credentials = credential;

                req.Method = method.ToUpper();
                req.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                if (headers != null)
                {
                    foreach (var key in headers.AllKeys)
                    {
                        var val = headers[key];
                        if (key == "Host")
                            req.Host = val;
                        else
                            req.Headers.Add(key, val);
                    }
                }
                return req.GetResponse() as HttpWebResponse;
            }
            catch (Exception e)
            {
                throw new RESTClientException(url, e);
            }

        }

        private HttpWebResponse SendData(string method, string url, object o, WebHeaderCollection headers)
        {
            var json = SerializeObject(o);
            var data = (new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)).GetBytes(json);

            try
            {
                var req = WebRequest.Create(url) as HttpWebRequest;
                req.Method = method;
                req.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                if (headers != null)
                {
                    foreach (var key in headers.AllKeys)
                    {
                        var val = headers[key];
                        if (key == "Host")
                            req.Host = val;
                        else
                            req.Headers.Add(key, val);
                    }
                }
                req.ContentLength = data.Length;
                req.ContentType = "application/x-www-form-urlencoded";
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

        private T DeserializeObject<T>(string s)
        {
            return UseISODates ? JsonConvert.DeserializeObject<T>(s, new IsoDateTimeConverter()) : JsonConvert.DeserializeObject<T>(s);
        }

        private string SerializeObject(object o)
        {
            return UseISODates ? JsonConvert.SerializeObject(o, new IsoDateTimeConverter()) : JsonConvert.SerializeObject(o);
        }

        internal static string ReadResponse(HttpWebResponse response)
        {
            var stream = response.GetResponseStream();
            if (stream == null)
                throw new Exception("Invalid response stream.");

            var encoding = String.IsNullOrEmpty(response.ContentEncoding)
                ? (String.IsNullOrEmpty(response.CharacterSet)
                    ? new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
                    : Encoding.GetEncoding(response.CharacterSet))
                : Encoding.GetEncoding(response.ContentEncoding);

            if (encoding == Encoding.UTF8)
            {
                return new StreamReader(stream).ReadToEnd();
            }

            var utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            return utf8.GetString(Encoding.Convert(encoding, utf8, encoding.GetBytes(new StreamReader(stream).ReadToEnd())));
        }
    }
}
