using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime;

namespace CK.Crs.Runtime.Formatting
{
    class JsonCommandBuilder : PipelineComponent
    {
        readonly IJsonConverter _jsonConverter;

        public JsonCommandBuilder( IJsonConverter jsonConverter )
        {
            _jsonConverter = jsonConverter;
        }

        public override bool ShouldInvoke( IPipeline pipeline )
        {
            return pipeline.Response == null && pipeline.Action.Description != null;
        }

        public override async Task Invoke( IPipeline pipeline, CancellationToken token )
        {
            if( pipeline.Action.Description == null ) throw new InvalidOperationException( "Cannot build a command without a valid description" );

            using( var reader = new StreamReader( pipeline.Request.Body ) )
            {
                string json = await reader.ReadToEndAsync();
                pipeline.Action.Command = _jsonConverter.ParseJson( json, pipeline.Action.Description.CommandType );
            }
            if( pipeline.Configuration.Events.CommandBuilt != null )
                await pipeline.Configuration.Events.CommandBuilt.Invoke( pipeline.Action );
        }

        protected virtual object CreateCommand( Type commandType )
        {
            return Activator.CreateInstance( commandType );
        }
    }
}
