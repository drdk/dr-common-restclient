using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Newtonsoft.Json;

namespace DR.Common.RESTClient
{
    [Serializable]
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

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            base.GetObjectData(info, context);

            info.AddValue(nameof(Json), Json);
        }
    }
}
