using CK.Core;
using System.Xml.Linq;

namespace CK.Crs.Scalability
{
    public class CommandResponseEnvelopeMetada
    {
        public string CallbackIdentifier { get; }

        public string PayloadContentType { get; }

        public string MonitorToken { get; }

        public CommandResponseEnvelopeMetada(string callbackId, string payloadContentType, IActivityMonitor monitor)
        {
            CallbackIdentifier = callbackId;
            PayloadContentType = payloadContentType;
            MonitorToken = monitor.DependentActivity().CreateToken().ToString();
        }

        public XElement ToXml()
        {
            return new XElement("Response",
                new XElement("Callback", CallbackIdentifier),
                new XElement("ContentType", PayloadContentType),
                new XElement("MonitorToken", MonitorToken)
            );
        }
    }
}
