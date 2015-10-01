using System;
using Newtonsoft.Json;

namespace DR.Common.RESTClient
{
    public class RESTSerializationException : JsonSerializationException
    {
        public string Json { get; set; }

        public RESTSerializationException(string data, string message, Exception e) : base(message, e)
        {
            if (!string.IsNullOrWhiteSpace(data))
            {
                Json = data;
            }
        }
    }
}
