using System;
using System.Net;

namespace DR.Common.RESTClient
{
    public class RESTClientException : Exception
    {
        public HttpStatusCode? StatusCode;
        public string StatusDescription;
        public string Content;
        public string Uri;

        public RESTClientException(string url, Exception exception)
            : base(exception.Message, exception)
        {
            Uri = url;

            var webException = exception as WebException;

            HttpWebResponse response = null;
            if (webException != null)
            {
                response = webException.Response as HttpWebResponse;
            }

            if (response != null)
            {
                try
                {
                    Content = RESTClient.ReadResponse(response);
                }
                catch
                {
                    Content = NO_CONTENT;
                }

                StatusCode = response.StatusCode;
                StatusDescription = response.StatusDescription;
            }
            else
            {
                StatusCode = null;
                StatusDescription = Message;
                Content = NO_CONTENT;
            }
        }

        private static string NO_CONTENT = "[[ unknown (this message is from RESTClientException, not actual content) ]]";
    }
}
