using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public class AmbientValuesFilter : ICommandFilter
    {
        readonly IAmbientValues _ambientValues;
        public AmbientValuesFilter( IAmbientValues ambientValues )
        {
            _ambientValues = ambientValues;
        }

        public int Order
        {
            get { return 0; }
        }

        public async Task OnCommandReceived( CommandContext context )
        {
            var monitor = context.ExecutionContext.Monitor;
            var model = context.ExecutionContext.Model;

            var result = await CheckValueAsync( monitor, _ambientValues, model);
            if( result == AmbientValueCheckResult.Failure )
            {
                string errorMessage = $"Identity check failed...";
                context.SetResult( new ValidationResult( errorMessage ) );
            }
        }

        protected virtual async Task<AmbientValueCheckResult> CheckValueAsync( IActivityMonitor monitor, IAmbientValues ambientValues, object model )
        {
            if( ambientValues == null ) throw new ArgumentNullException( nameof( ambientValues ) );
            if( model == null ) throw new ArgumentNullException( nameof( model ) );

            Type modelType = model.GetType();
            using( monitor.OpenTrace().Send( "Identity checking" ) )
            {
                monitor.Trace().Send( $"[For type {modelType}]" );

                dynamic dModel = model;
                int? actorId = dModel.ActorId;
                if( actorId.HasValue )
                {
                    int ambientActorId = await ambientValues.GetValueAsync<int>( "ActorId" );
                    if( !ambientActorId.Equals( actorId.Value ) )
                    {
                        return AmbientValueCheckResult.Failure;
                    }
                }

                int? authenticatedActorId = dModel.AuthenticatedActorId;
                if( authenticatedActorId.HasValue )
                {
                    int ambientAuthenticatedActorId = await ambientValues.GetValueAsync<int>( "AuthenticatedActorId" );
                    if( !ambientAuthenticatedActorId.Equals( actorId.Value ) )
                    {
                        return AmbientValueCheckResult.Failure;
                    }
                }

                return AmbientValueCheckResult.Success;
            }
        }
    }
}
