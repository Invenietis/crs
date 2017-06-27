using CK.Core;
using System;
using System.Xml.Linq;

namespace CK.Crs.Scalability
{
    public class CommandJobMetada
    {
        public Type CommandType { get; }

        public string CallbackId { get; }

        public string ContentType { get; }

        public string MonitorToken { get; }

        public CommandJobMetada( XElement root)
        {
            CommandType =CK.Core.SimpleTypeFinder.WeakResolver(root.Element("CommandType").Value, true );
            CallbackId = root.Element("CallbackId").Value;
            ContentType = root.Element("ContentType").Value;
            MonitorToken = root.Element("MonitorToken").Value;
        }

        public CommandJobMetada( IActivityMonitor monitor, CommandAction commandAction)
        {
            CommandType = commandAction.Description.CommandType;
            CallbackId = commandAction.CallbackId;
            ContentType = "application/json";
            MonitorToken = monitor.DependentActivity().CreateToken().ToString();
        }

        public XElement ToXml()
        {
            return new XElement("Metadata",
                new XElement("CommandType", CommandType.AssemblyQualifiedName),
                new XElement("CallbackId", CallbackId),
                new XElement("ContentType", ContentType),
                new XElement("MonitorToken", MonitorToken)
            );
        }
    }
}
