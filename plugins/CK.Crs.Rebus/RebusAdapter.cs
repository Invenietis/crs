using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CK.Core;
using R = Rebus.Bus;

namespace CK.Crs
{
    public class RebusAdapter : ICommandDispatcher, IEventPublisher, IDisposable
    {
        readonly R.IBus _bus;

        public RebusAdapter( R.IBus bus )
        {
            _bus = bus;
        }

        public void Dispose()
        {
            _bus.Dispose();
        }

        public Task PostAsync<T>( T command, ICommandContext context ) where T : class
        {
            var h = ToHeaders( context );
            return _bus.Send( command, h );
        }

        public Task PublishAsync<T>( T evt, ICommandContext context ) where T : class
        {
            var h = ToHeaders( context );
            return _bus.Publish( evt, h );
        }

        private Dictionary<string, string> ToHeaders( ICommandContext context )
        {
            return new Dictionary<string, string> {
                { nameof( ICommandContext.CallerId ), context.CallerId },
                { nameof( ICommandContext.CommandId ), context.CommandId.ToString() },
                { nameof( ICommandContext.Monitor ), context.Monitor.DependentActivity().CreateToken().ToString() },
            };
        }
    }
}
