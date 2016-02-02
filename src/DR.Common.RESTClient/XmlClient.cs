using System.IO;
using System.Xml.Serialization;

namespace DR.Common.RESTClient
{
    public class XmlClient : RESTClient, IXmlClient
    {
        public XmlClient()
        {
            ContentType = "application/xml";
            Accept = "application/xml, text/xml, */*; q=0.01";
        }
        public override T DeserializeObject<T>(string s)
        {
            var xs = new XmlSerializer(typeof(T));
            using (var sr = new StringReader(s))
                return (T) xs.Deserialize(sr);
        }

        protected override string SerializeObject(object o)
        {
            var xs = new XmlSerializer(o.GetType());
            using (var sw = new StringWriter())
            {
                xs.Serialize(sw, o);
                return sw.ToString();
            }
        }
    }
}
