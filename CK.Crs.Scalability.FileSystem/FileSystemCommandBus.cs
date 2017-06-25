using CK.Core;
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
        readonly CKTrait _componentTrait;
        public FileSystemCommandBus( IJsonConverter jsonConverter, FileSystemConfiguration configuration, CKTrait trait )
        {
            _converter = new CommandConverter(jsonConverter);
            _configuration = configuration;
            _componentTrait = trait;
        }

        public override async Task Invoke(IPipeline pipeline, CancellationToken token = default(CancellationToken))
        {
            await _converter.WriteCommand(pipeline.Monitor, pipeline.Action, _configuration.Path);

            var response = new ScalableCommandResponse( pipeline.Action );
            pipeline.Response.Set(response);
        }

        public override bool ShouldInvoke(IPipeline pipeline)
        {
            return !pipeline.Response.HasReponse && _componentTrait.Overlaps(  pipeline.Action.Description.Traits );
        }
    }
}
