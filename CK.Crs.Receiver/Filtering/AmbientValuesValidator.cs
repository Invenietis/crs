using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Filtering;

namespace CK.Crs.Runtime.Filtering
{
    class AmbientValuesValidator : PipelineComponent
    {
        readonly IAmbientValues _ambientValues;

        public AmbientValuesValidator( IAmbientValues ambientValues )
        {
            if( ambientValues == null ) throw new ArgumentNullException( nameof( ambientValues ) );
            _ambientValues = ambientValues;
        }

        /// <summary>
        /// The command should have been binded
        /// </summary>
        public override bool ShouldInvoke( IPipeline pipeline )
        {
            return pipeline.Response == null && pipeline.Action.Command != null;
        }

        public override async Task Invoke( IPipeline pipeline, CancellationToken token )
        {
            using( pipeline.Monitor.OpenTrace().Send("Ambient values validation..." ) )
            {
                var context = new ReflectionAmbientValueValidationContext( pipeline.Monitor, pipeline.Action, _ambientValues );
                if( pipeline.Configuration.Events.AmbientValuesValidating != null )
                    await pipeline.Configuration.Events.AmbientValuesValidating?.Invoke( context );

                if( context.Rejected ) pipeline.Monitor.Info().Send("Validation failed by custom processing in Pipeline.Events.ValidatingAmbientValues." );
                else
                {
                    await context.ValidateValueAndRejectOnError<int>( "ActorId" );
                    await context.ValidateValueAndRejectOnError<int>( "AuthenticatedActorId" );
                }
                if( pipeline.Configuration.Events.AmbientValuesValidated != null )
                    await pipeline.Configuration.Events.AmbientValuesValidated?.Invoke( context );

                if( context.Rejected )
                {
                    var rejectedContext = new CancellableCommandRejectedContext( pipeline.Action, context);
                    if( pipeline.Configuration.Events.CommandRejected != null )
                        await pipeline.Configuration.Events.CommandRejected?.Invoke( rejectedContext );

                    if( rejectedContext.Canceled == false )
                        SetInvalidAmbientValuesResponse( pipeline, context );
                }
            }
        }


        private void SetInvalidAmbientValuesResponse( IPipeline pipeline, AmbientValueValidationContext context )
        {
            if( context.Rejected )
            {
                string msg =  $"Invalid ambient values detected: {context.RejectReason}";
                pipeline.Monitor.Warn().Send( msg );
                pipeline.Response = new CommandErrorResponse( msg, pipeline.Action.CommandId );
            }
            else
            {
                pipeline.Monitor.Info().Send("Ambient values validator invalidate ambient values, but the last hook from Pipeline.Events.AmbientValuesInvalidated cancel the rejection." );
            }
        }
    }
}
