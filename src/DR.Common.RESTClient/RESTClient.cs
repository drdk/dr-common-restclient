using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;

namespace DR.Common.RESTClient
{
    public abstract class RESTClient : IRESTClient
    {
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
            string response = Request(method, url, credential, headers, useDefaultCredentials);
            return DeserializeObject<T>(response);
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
        public string Get(string url, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false)
        {
            return Request("GET", url, credential, headers, useDefaultCredentials);
        }
        public T Get<T>(string url, NetworkCredential credential = null, WebHeaderCollection headers = null,
            bool useDefaultCredentials = false) where T : class
        {
            return Request<T>("GET", url, credential, headers, useDefaultCredentials);
        }
        public string Delete(string url, WebHeaderCollection headers = null)
        {
            return Request("DELETE", url, headers: headers);
        }
        public T Delete<T>(string url, WebHeaderCollection headers = null) where T : class
        {
            return Request<T>("DELETE", url, headers);
        }
        public string Post(string url, object o, WebHeaderCollection headers = null)
        {
            return Request("POST", url, o, headers);
        }
        public T Post<T>(string url, object o, WebHeaderCollection headers = null) where T : class
        {
            return Request<T>("POST", url, o, headers);
        }
        public string Put(string url, object o, WebHeaderCollection headers = null)
        {
            return Request("PUT", url, o, headers);
        }
        public T Put<T>(string url, object o, WebHeaderCollection headers = null) where T : class
        {
            return Request<T>("PUT", url, o, headers);
        }
        public T DeserializeContent<T>(RESTClientException exception)
        {
            return DeserializeObject<T>(exception.Content);
        }
        public abstract T DeserializeObject<T>(string s);

        protected abstract string SerializeObject(object o);

        private HttpWebResponse RequestData(string method, string url, NetworkCredential credential, WebHeaderCollection headers, bool useDefaultCredentials)
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
                req.Accept = Accept;

                return req.GetResponse() as HttpWebResponse;
            }
            catch (Exception e)
            {
                throw new RESTClientException(url, e);
            }

        }
        protected HttpWebResponse SendData(string method, string url, object o, WebHeaderCollection headers)
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
                var text = SerializeObject(o);
                data = (new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)).GetBytes(text);
            }

            try
            {
                var req = PrepareHttpWebRequest(url, method, headers);

                req.ContentLength = data.Length;
                req.ContentType = ContentType;
                req.Accept = Accept;

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

        protected HttpWebRequest PrepareHttpWebRequest(string url, string method, WebHeaderCollection headers)
        {
            var req = HttpWebRequest.Create(url) as HttpWebRequest;

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

            return req;
        }

        protected string _baseUrl;
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
        public string ContentType { get; set; }
        public string Accept { get; set; } 
    }
}
