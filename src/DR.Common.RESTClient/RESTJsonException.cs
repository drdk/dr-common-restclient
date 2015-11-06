using System;
using Newtonsoft.Json;

namespace DR.Common.RESTClient
{
    public class RESTJsonException : JsonException
    {
        public string Json { get; set; }

        public RESTJsonException(string data, string message, Exception e) : base(message, e)
        {
            if (!string.IsNullOrWhiteSpace(data))
            {
                Json = data;
            }
        }
    }
}
