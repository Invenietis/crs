﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using CK.Core;

namespace CK.Crs.Runtime.Pipeline
{
    class JsonCommandBuilder : PipelineComponent
    {
        readonly JsonSerializer _serializer;

        public JsonCommandBuilder( IPipeline pipeline ) : base( pipeline )
        {
            _serializer = Newtonsoft.Json.JsonSerializer.Create( new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            } );
        }

        public override bool ShouldInvoke
        {
            get { return Pipeline.Response == null && Pipeline.Action.Description != null; }
        }

        public override async Task Invoke( CancellationToken token )
        {
            if( Pipeline.Action.Description == null ) throw new InvalidOperationException( "Cannot build a command without a valid description" );

            Pipeline.Action.Command = CreateCommand( Pipeline.Action.Description.Descriptor.CommandType );
            if( Pipeline.Action.Command == null )
            {
                string msg = String.Format(
                        "A valid command definition has been infered from routes, but the command type {0} failed to be instanciated.",
                        Pipeline.Action.Description.Descriptor.CommandType.Name );
                Pipeline.Monitor.Error().Send( msg );
            }
            else
            {
                using( var reader = new StreamReader( Pipeline.Request.Body ) )
                {
                    _serializer.Populate( reader, Pipeline.Action.Command );
                }
                if( Pipeline.Events.CommandBuilt != null )
                    await Pipeline.Events.CommandBuilt?.Invoke( Pipeline.Action );
            }
        }

        protected virtual object CreateCommand( Type commandType )
        {
            return Activator.CreateInstance( commandType );
        }
    }
}
