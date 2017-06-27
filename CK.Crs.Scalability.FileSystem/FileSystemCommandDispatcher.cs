using CK.Core;
using CK.Crs.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace CK.Crs.Scalability.FileSystem
{
    class FileSystemCommandDispatcher : ICommandResponseDispatcher
    {
        readonly FileSystemConfiguration _configuration;

        public FileSystemCommandDispatcher(FileSystemConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task DispatchAsync(IActivityMonitor monitor, string callbackId, CommandResponseBuilder response, CancellationToken cancellationToken = default(CancellationToken))
        {
            string contentType = "application/octet-stream";
            if (response.Headers.ContainsKey(CommandResponseHeaders.ContentType))
            {
                contentType = response.Headers[CommandResponseHeaders.ContentType];
            }
            var metaData = new CommandResponseMetada(callbackId, contentType, monitor);

            var payloadPath = _configuration.FormatResponsePath(response.CommandResponse);
            var metaDataPath = payloadPath + ".xml";

            using (var fs = File.OpenWrite(payloadPath))
            {
                await response.WriteAsync(fs);
            }

            using (var fs = File.OpenWrite(metaDataPath))
            {
                using (var xmlW = XmlWriter.Create(fs, new XmlWriterSettings
                {
                    Async = true
                }))
                {
                    metaData.ToXml().WriteTo(xmlW);
                }
            }
        }
    }
}
