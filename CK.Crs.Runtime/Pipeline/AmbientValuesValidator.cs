using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Pipeline
{
    class AmbientValuesValidator : PipelineSlotBase
    {
        readonly IAmbientValues _ambientValues;

        public AmbientValuesValidator( CommandReceivingPipeline pipeline, IAmbientValues ambientValues ) : base( pipeline )
        {
            if( ambientValues == null ) throw new ArgumentNullException( nameof( ambientValues ) );
            _ambientValues = ambientValues;
        }

        public override bool ShouldInvoke
        {
            get { return Pipeline.Response == null && Pipeline.Request.Command != null; }
        }

        public override async Task Invoke( CancellationToken token )
        {
            if( ShouldInvoke )
            {
                Type modelType = Pipeline.Request.Command.GetType();
                using( Monitor.OpenTrace().Send( "Identity checking" ) )
                {
                    Monitor.Trace().Send( $"[For type {modelType}]" );

                    dynamic dModel = model;
                    int? actorId = dModel.ActorId;
                    if( actorId.HasValue )
                    {
                        int ambientActorId = await _ambientValues.GetValueAsync<int>( "ActorId" );
                        if( !ambientActorId.Equals( actorId.Value ) )
                        {
                            await SetInvalidAmbientValuesResponse();
                        }
                    }

                    int? authenticatedActorId = dModel.AuthenticatedActorId;
                    if( authenticatedActorId.HasValue )
                    {
                        int ambientAuthenticatedActorId = await _ambientValues.GetValueAsync<int>( "AuthenticatedActorId" );
                        if( !ambientAuthenticatedActorId.Equals( actorId.Value ) )
                        {
                            await SetInvalidAmbientValuesResponse();
                        }
                    }
                }
            }
        }

        private Task SetInvalidAmbientValuesResponse()
        {
            Pipeline.Response = new CommandErrorResponse( "Invalid ambient values", Pipeline.CommandId );
            if( Pipeline.Events.AmbientValuesInvalid != null )
                return Pipeline.Events.AmbientValuesInvalid( _ambientValues );

            return Task.FromResult( 0 );
        }
    }
}
