using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using System.Reflection;

namespace CK.Crs.Runtime.Pipeline
{
    class AmbientValuesValidator : PipelineComponent
    {
        readonly IAmbientValues _ambientValues;

        public AmbientValuesValidator( IPipeline pipeline, IAmbientValues ambientValues ) : base( pipeline )
        {
            if( ambientValues == null ) throw new ArgumentNullException( nameof( ambientValues ) );
            _ambientValues = ambientValues;
        }

        /// <summary>
        /// The command should have been binded
        /// </summary>
        public override bool ShouldInvoke
        {
            get { return Pipeline.Response == null && Pipeline.Action.Command != null; }
        }

        public override async Task Invoke( CancellationToken token )
        {
            using( Monitor.OpenTrace().Send( "Ambient values validation..." ) )
            {
                var context = new ReflectionAmbientValueValidationContext( Pipeline.Monitor, Pipeline.Action, _ambientValues );
                await Pipeline.Events.AmbientValuesValidating?.Invoke( context );

                if( context.Rejected ) Monitor.Info().Send( "Validation failed by custom processing in Pipeline.Events.ValidatingAmbientValues." );
                else
                {
                    await context.ValidateValueAndRejectOnError<int>( "ActorId" );
                    await context.ValidateValueAndRejectOnError<int>( "AuthenticatedActorId" );
                }
                await Pipeline.Events.AmbientValuesValidated?.Invoke( context );

                if( context.Rejected )
                {
                    var rejectedContext = new CancellableCommandRejectedContext( Pipeline.Action, context);
                    await Pipeline.Events.CommandRejected?.Invoke( rejectedContext );

                    if( rejectedContext.Canceled == false )
                        SetInvalidAmbientValuesResponse( context );
                }
            }
        }


        private void SetInvalidAmbientValuesResponse( AmbientValueValidationContext context )
        {
            if( context.Rejected )
            {
                string msg =  $"Invalid ambient values detected: {context.RejectReason}";
                Monitor.Warn().Send( msg );
                Pipeline.Response = new CommandErrorResponse( msg, Pipeline.Action.CommandId );
            }
            else
            {
                Monitor.Info().Send( "Ambient values validator invalidate ambient values, but the last hook from Pipeline.Events.AmbientValuesInvalidated cancel the rejection." );
            }
        }

        protected class ReflectionAmbientValueValidationContext : AmbientValueValidationContext
        {
            public IReadOnlyCollection<PropertyInfo> Properties { get; }

            public ReflectionAmbientValueValidationContext( IActivityMonitor monitor, CommandAction action, IAmbientValues ambientValues ) : base( monitor, action, ambientValues )
            {
                Properties = action.Description.Descriptor.CommandType.GetRuntimeProperties().ToArray();
            }

            public sealed override async Task<bool> ValidateValue<T>( string valueName, AmbientValueComparer<T> comparer )
            {
                var property = Properties.FirstOrDefault( x => x.Name == valueName);
                if( property == null ) Monitor.Info().Send( "Property {0} not found for value {1} on command {2}", valueName, valueName, Action.Description.Descriptor.CommandType.Name );
                else
                {
                    T value = (T) property.GetMethod.Invoke( Action.Command, null );
                    Monitor.Trace().Send( "Getting {0} by reflection on the command and obtained {1}.", property.Name, value != null ? value.ToString() : "<NULL>" );
                    T ambientValue = await AmbientValues.GetValueAsync<T>( valueName);
                    Monitor.Trace().Send( "Getting {0} in the ambient values and obtained {1}.", valueName, ambientValue != null ? ambientValue.ToString() : "<NULL>" );
                    return comparer( valueName, value, ambientValue );
                }
                return true;
            }
        }
    }
}
