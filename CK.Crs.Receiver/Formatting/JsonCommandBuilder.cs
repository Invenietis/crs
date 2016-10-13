using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using CK.Core;
using CK.Crs.Runtime;

namespace CK.Crs.Runtime.Formatting
{
    class JsonCommandBuilder : PipelineComponent
    {
        readonly JsonSerializer _serializer;

        public JsonCommandBuilder()
        {
            _serializer = JsonSerializer.Create( new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            } );
        }

        public override bool ShouldInvoke( IPipeline pipeline )
        {
            return pipeline.Response == null && pipeline.Action.Description != null;
        }

        public override async Task Invoke( IPipeline pipeline, CancellationToken token )
        {
            if( pipeline.Action.Description == null ) throw new InvalidOperationException( "Cannot build a command without a valid description" );

            pipeline.Action.Command = CreateCommand( pipeline.Action.Description.CommandType );
            if( pipeline.Action.Command == null )
            {
                string msg = String.Format(
                        "A valid command definition has been infered from routes, but the command type {0} failed to be instanciated.",
                        pipeline.Action.Description.CommandType.Name );
                pipeline.Monitor.Error().Send( msg );
            }
            else
            {
                using( var reader = new StreamReader( pipeline.Request.Body ) )
                {
                    _serializer.Populate( reader, pipeline.Action.Command );
                }
                if( pipeline.Configuration.Events.CommandBuilt != null )
                    await pipeline.Configuration.Events.CommandBuilt.Invoke( pipeline.Action );
            }
        }

        protected virtual object CreateCommand( Type commandType )
        {
            return Activator.CreateInstance( commandType );
        }
    }
}
