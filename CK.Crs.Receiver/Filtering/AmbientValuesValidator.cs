using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using Microsoft.Extensions.Caching.Memory;

namespace CK.Crs.Runtime.Filtering
{
    class AmbientValuesValidator : PipelineComponent
    {
        readonly IAmbientValues _ambientValues;
        readonly IMemoryCache _memoryCache;
        readonly IAmbientValuesRegistration _ambientValueRegistration;

        public AmbientValuesValidator(IAmbientValuesRegistration ambientValueRegistration, IAmbientValues ambientValues, IMemoryCache memoryCache )
        {
            _ambientValueRegistration = ambientValueRegistration;
            _ambientValues = ambientValues ?? throw new ArgumentNullException( nameof( ambientValues ) );
            _memoryCache = memoryCache ?? throw new ArgumentNullException( nameof( memoryCache ) );
        }

        /// <summary>
        /// The command should have been binded
        /// </summary>
        public override bool ShouldInvoke( IPipeline pipeline )
        {
            return pipeline.Response.HasReponse == false && pipeline.Action.Command != null;
        }

        public override async Task Invoke( IPipeline pipeline, CancellationToken token )
        {
            using( pipeline.Monitor.OpenTrace().Send("Ambient values validation..." ) )
            {
                var context = new ReflectionAmbientValueValidationContext( 
                    pipeline.Monitor, 
                    pipeline.Action, 
                    _ambientValues, 
                    _memoryCache );

                if( pipeline.Configuration.Events.AmbientValuesValidating != null )
                    await pipeline.Configuration.Events.AmbientValuesValidating?.Invoke( context );

                if( context.Rejected ) pipeline.Monitor.Info().Send("Validation failed by custom processing in Pipeline.Events.ValidatingAmbientValues." );
                else
                {
                    foreach( var v in _ambientValueRegistration.AmbientValues)
                    {
                        await context.ValidateValueAndRejectOnError<int>( v.Name );
                    }
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
                pipeline.Response.Set( new CommandErrorResponse( msg, pipeline.Action.CommandId ) );
            }
            else
            {
                pipeline.Monitor.Info().Send("Ambient values validator invalidate ambient values, but the last hook from Pipeline.Events.AmbientValuesInvalidated cancel the rejection." );
            }
        }
    }
}
