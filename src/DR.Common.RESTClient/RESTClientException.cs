using System;
using System.Net;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace DR.Common.RESTClient
{
    [Serializable]
    public class RESTClientException: Exception
    {
        public virtual HttpStatusCode? StatusCode { get; private set; }
        public virtual string StatusDescription { get; private set; }
        public virtual string Content { get; private set; }
        public string Uri { get; private set; }

        private readonly string _message;
        public override string Message { get { return _message; } }

        private RESTClientException(SerializationInfo info, StreamingContext context) { } // need for unit test mocking ?

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
                    Content = NoContent;
                }

                StatusCode = response.StatusCode;
                StatusDescription = response.StatusDescription;
                _message = (!string.IsNullOrEmpty(response.StatusDescription) && !string.IsNullOrEmpty(Uri) ?
                    response.StatusDescription + " Uri : \"" + Uri + "\". Inner message : \"" + exception.Message + "\"" :
                    exception.Message);
            }
            else
            {
                StatusCode = null;
                StatusDescription = null;
                Content = NoContent;
                _message = exception.Message;
            }
        }

        public T DeserializeContent<T>(params JsonConverter[] jsonConverters)
        {
            return JsonConvert.DeserializeObject<T>(Content, jsonConverters);
        }

        private const string NoContent = "[[ unknown (this message is from RESTClientException, not actual content) ]]";

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            base.GetObjectData(info,context);

            info.AddValue("Content", Content);
            info.AddValue("StatusCode", StatusCode);
            info.AddValue("StatusDescription", StatusDescription);
            info.AddValue("Uri", Uri);
        }
    }
}
