using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    public class DefaultBus : IBus
    {
        readonly ICommandDispatcher _dispatcher;
        readonly IEventPublisher _publisher;
        readonly ICommandSender _sender;

        public DefaultBus( IServiceProvider services )
        {
            _sender = services.GetRequiredService<ICommandSender>();
            _publisher = services.GetRequiredService<IEventPublisher>();
            _dispatcher = services.GetService<ICommandDispatcher>();
        }

        public Task<object> SendAsync<T>( T command, ICommandContext context ) where T : class
        {
            return _sender.SendAsync( command, context );
        }

        public Task PostAsync<T>( T command, ICommandContext context ) where T : class
        {
            if( _dispatcher == null ) throw new InvalidOperationException( "You must use a Crs Plugins supporting command dispatching. See Rebus for example." );
            return _dispatcher.PostAsync( command, context );
        }

        public Task PublishAsync<T>( T evt, ICommandContext context ) where T : class
        {
            return _publisher.PublishAsync( evt, context );
        }
    }
}
