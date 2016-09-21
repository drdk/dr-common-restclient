using System.Net;

namespace DR.Common.RESTClient
{
    public interface IJsonClient
    {
        string Request(string method, string url, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false);
        T Request<T>(string method, string url, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false) where T : class;
        string Request(string method, string url, object o, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false);
        T Request<T>(string method, string url, object o, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false) where T : class;

        string Get(string url, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false);
        T Get<T>(string url, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false) where T : class;

        string Delete(string url, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false);
        T Delete<T>(string url, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false) where T : class;

        string Post(string url, object o, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false);
        T Post<T>(string url, object o, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false) where T : class;

        string Put(string url, object o, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false);
        T Put<T>(string url, object o, NetworkCredential credential = null, WebHeaderCollection headers = null, bool useDefaultCredentials = false) where T : class;
        string BaseURL { get; set; }
        T DeserializeContent<T>(RESTClientException exception);
        T DeserializeObject<T>(string s);
        string ContentType { get; set; }
    }
}
