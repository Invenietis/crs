using CK.Crs.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.Scalability.FileSystem
{
    class FileSystemCommandBus : PipelineComponent
    {
        readonly CommandConverter _converter;
        readonly FileSystemConfiguration _configuration;
        public FileSystemCommandBus( IJsonConverter jsonConverter, FileSystemConfiguration configuration )
        {
            _converter = new CommandConverter(jsonConverter);
            _configuration = configuration;
        }

        public override Task Invoke(IPipeline pipeline, CancellationToken token = default(CancellationToken))
        {
            return _converter.WriteCommand(pipeline.Monitor, pipeline.Action, _configuration.Path);
        }

        public override bool ShouldInvoke(IPipeline pipeline)
        {
            return !pipeline.Response.HasReponse && pipeline.Action.Description.Traits.Contains(Traits.Scalable);
        }
    }
}
