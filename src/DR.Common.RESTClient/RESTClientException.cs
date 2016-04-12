using System;
using System.Net;

namespace DR.Common.RESTClient
{
    public class RESTClientException: Exception
    {
        public virtual HttpStatusCode? StatusCode { get; private set; }
        public virtual string StatusDescription { get; private set; }
        public virtual string Content { get; private set; }
        public string Uri { get; private set; }

        private readonly string _message;
        public override string Message { get { return _message; } }

        public RESTClientException() { }

        public RESTClientException(string url=null, HttpStatusCode? statusCode = null)
        {
            Uri = url;
            StatusCode = statusCode;
        }

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
                _message = (!string.IsNullOrEmpty(StatusDescription) && !string.IsNullOrEmpty(Uri) ?
                    StatusDescription + " Uri : \"" + Uri + "\". Inner message : \"" + exception.Message + "\"" :
                    exception.Message);
            }
            else
            {
                StatusCode = null;
                StatusDescription = null;
                Content = NO_CONTENT;
                _message = exception.Message;
            }
        }

        private static string NO_CONTENT = "[[ unknown (this message is from RESTClientException, not actual content) ]]";
    }
}
