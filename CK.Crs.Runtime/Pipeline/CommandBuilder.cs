using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CK.Crs.Runtime.Pipeline
{
    class CommandBuilder : PipelineSlotBase
    {
        readonly JsonSerializer _serializer;

        public CommandBuilder( CommandReceivingPipeline pipeline ) : base( pipeline )
        {
            _serializer = Newtonsoft.Json.JsonSerializer.Create( new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            } );
        }

        public override bool ShouldInvoke
        {
            get { return Pipeline.Response == null && Pipeline.Request.CommandDescription != null; }
        }

        public override Task Invoke( CancellationToken token )
        {
            if( ShouldInvoke )
            {
                if( Pipeline.Request.CommandDescription == null ) throw new InvalidOperationException( "Cannot build a command without a valid description" );

                Pipeline.Request.Command = CreateCommand( Pipeline.Request.CommandDescription.Descriptor.CommandType );
                using( var reader = new StreamReader( Pipeline.Request.Body ) )
                {
                    _serializer.Populate( reader, Pipeline.Request.Command );
                }

                if( Pipeline.Request.Command != null )
                {
                    string msg = String.Format(
                        "A valid command definition has been infered from routes, but the command type {0} failed to be instanciated.",
                        Pipeline.Request.CommandDescription.Descriptor.CommandType.Name );
                    throw new InvalidOperationException( msg );
                }
            }

            return Task.FromResult( 0 );
        }

        protected virtual object CreateCommand( Type commandType )
        {
            return Activator.CreateInstance( commandType );
        }
    }
}
