using System;
using System.Net;

namespace DR.Common.RESTClient
{
    public class XmlClient : RESTClient, IXmlClient
    {
        public override T DeserializeContent<T>(RESTClientException exception)
        {
            throw new NotImplementedException();
        }

        protected override HttpWebResponse RequestData(string method, string url, NetworkCredential credential, WebHeaderCollection headers, bool useDefaultCredentials)
        {
            throw new NotImplementedException();
        }

        protected override HttpWebResponse SendData(string method, string url, object o, WebHeaderCollection headers)
        {
            throw new NotImplementedException();
        }

        protected override T DeserializeObject<T>(string s)
        {
            throw new NotImplementedException();
        }
    }
}
