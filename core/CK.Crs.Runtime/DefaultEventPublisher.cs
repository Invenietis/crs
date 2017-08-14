﻿using CK.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class DefaultEventPublisher : IEventPublisher
    {
        readonly IRequestRegistry _registry;
        readonly IRequestHandlerFactory _factory;
        public DefaultEventPublisher( IRequestRegistry registry, IRequestHandlerFactory factory )
        {
            _registry = registry;
            _factory = factory;
        }

        public async Task PublishAsync<T>( T evt, ICommandContext context ) where T : class
        {
            var all = _registry.Registration.Where( c => c.Type == typeof( T ) );
            foreach( var desc in all ) await DoSendAsync( evt, context, desc );
        }

        private async Task DoSendAsync<T>( T command, ICommandContext context, RequestDescription desc ) where T : class
        {
            if( desc == null ) throw new ArgumentException( String.Format( "Command {0} not registered", typeof( T ).Name ) );

            IEventHandler<T> handler = _factory.CreateHandler( desc.HandlerType ) as IEventHandler<T>;
            if( handler == null ) throw new ArgumentException( String.Format( "Handler {0} for {1} impossible to created", desc.HandlerType ) );
            try
            {
                await handler.HandleAsync( command, context );
            }
            catch( Exception ex )
            {
                context.Monitor.Error( ex );
            }
            finally
            {
                _factory.ReleaseHandler( handler );
            }
        }
    }
}
