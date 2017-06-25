using CK.Core;
using CK.Crs.Runtime;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CK.Crs.Scalability.FileSystem
{
    class CommandConverter
    {
        readonly IJsonConverter _jsonConverter;
        public CommandConverter(IJsonConverter jsonConverter)
        {
            _jsonConverter = jsonConverter;
        }

        public async Task WriteCommand(IActivityMonitor monitor, CommandAction commandAction, string folderFullPath)
        {
            var metaData = new CommandJobMetada(monitor, commandAction);
            var commandName = commandAction.CommandId.ToString("N");

            // 1. Writes first the payload
            var commandPayloadPath = Path.Combine(folderFullPath, commandName + ".command");
            var json = _jsonConverter.ToJson(commandAction.Command);
            File.WriteAllBytes(commandPayloadPath, Encoding.UTF8.GetBytes(json));

            // 2. Then writes the metada. The watcher of the command receiver is watching for metadata file.
            var metadataPath = Path.Combine(folderFullPath, commandName + ".command.xml");
            using (var fs = File.OpenWrite(metadataPath))
            using (var xmlWriter = XmlWriter.Create(fs))
            {
                metaData.ToXml().WriteTo(xmlWriter);
                await xmlWriter.FlushAsync();
            }
        }

        public CommandJob ReadCommand(IActivityMonitor monitor, string fullPath)
        {
            var metaData = new CommandJobMetada(XElement.Load(fullPath));
            var commandFilePath = fullPath.Remove(fullPath.Length - 5, 4);
            if (!File.Exists(commandFilePath))
            {
                monitor.Warn().Send("No command found for the metadata {0}", fullPath);
                return null;
            }

            var enveloppe = new CommandJob(monitor, metaData, File.ReadAllBytes(commandFilePath));

            return enveloppe;
        }
    }
}
