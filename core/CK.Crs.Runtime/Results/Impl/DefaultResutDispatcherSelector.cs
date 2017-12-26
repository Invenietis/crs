using CK.Core;
using CK.Crs.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace CK.Crs.Results
{
    class DefaultResutDispatcherSelector : IResultDispatcherSelector
    {
        readonly IServiceProvider _services;
        readonly IOptions<ResultDispatcherOptions> _options;

        public DefaultResutDispatcherSelector( IServiceProvider services, IOptions<ResultDispatcherOptions> options )
        {
            _services = services;
            _options = options;
        }

        public IResultDispatcher SelectDispatcher( ICommandContext context )
        {
            using( context.Monitor.OpenTrace( "Selecting the appropriate result dispatcher..." ) )
            {
                context.Monitor.Trace( $"Given the provided protocol {context.CallerId.Protocol}..." );
                if( _options.Value.Dispatchers.TryGetValue( context.CallerId.Protocol, out Type resultDispatcherType ) )
                {
                    if( !typeof( IResultDispatcher ).IsAssignableFrom( resultDispatcherType ) )
                        throw new InvalidOperationException( "The result dispatcher does not match the IResultDispatcher. Something is wrong!" );

                    context.Monitor.Trace( $"...found a valid result dispatcher of type {resultDispatcherType.FullName}" );
                    return (IResultDispatcher)_services.GetService( resultDispatcherType ) ?? (IResultDispatcher)ActivatorUtilities.CreateInstance( _services, resultDispatcherType );
                }

                context.Monitor.Warn( "... no result dispatcher found." );
                return NoopResultDispatcher.Instance;
            }
        }
    }
}
