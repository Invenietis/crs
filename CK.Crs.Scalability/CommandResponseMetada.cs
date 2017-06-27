using CK.Core;
using System.Xml.Linq;

namespace CK.Crs.Scalability
{
    public class CommandResponseMetada
    {
        public string Callback { get; }

        public string ContentType { get; }

        public string MonitorToken { get; }

        public CommandResponseMetada(string callbackId, string contentType, IActivityMonitor monitor)
        {
            Callback = callbackId;
            ContentType = contentType;
            MonitorToken = monitor.DependentActivity().CreateToken().ToString();
        }

        public XElement ToXml()
        {
            return new XElement("Response",
                new XElement("Callback", Callback),
                new XElement("ContentType", ContentType),
                new XElement("MonitorToken", MonitorToken)
            );
        }
    }
}
