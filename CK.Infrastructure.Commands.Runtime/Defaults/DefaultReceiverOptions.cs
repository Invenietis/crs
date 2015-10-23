using System;
namespace CK.Infrastructure.Commands
{
    public class DefaultReceiverOptions : ICommandReceiverOptions
    {
        ICommandRouteMap _map;
        ICommandHandlerRegistry _registry;

        public DefaultReceiverOptions( string routePrefix, ICommandRouteMap map, ICommandHandlerRegistry registry )
        {
            if( String.IsNullOrWhiteSpace( routePrefix ) ) throw new ArgumentNullException( nameof( routePrefix ) );
            if( !routePrefix.StartsWith( "/" ) ) routePrefix = '/' + routePrefix;

            RoutePrefix = routePrefix;
            _map = map;
            _registry = registry;
        }

        public ICommandReceiverOptions Register<TCommand, THandler>( string route, bool isLongRunning )
            where TCommand : class
            where THandler : class, ICommandHandler
        {
            _map.Register( new CommandRouteRegistration
            {
                CommandType = typeof( TCommand ),
                Route = new CommandRoutePath( RoutePrefix, route ),
                IsLongRunningCommand = isLongRunning
            } );

            _registry.RegisterHandler<TCommand, THandler>();
            return this;
        }

        public string RoutePrefix { get; private set; }
    }
}