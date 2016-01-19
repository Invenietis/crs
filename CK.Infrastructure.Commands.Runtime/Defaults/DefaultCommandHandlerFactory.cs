using System;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandHandlerFactory : ICommandHandlerFactory
    {
        readonly DefaultCreateInstanceStrategy _s;
        public DefaultCommandHandlerFactory( IServiceProvider serviceProvider )
        {
            _s = new DefaultCreateInstanceStrategy( serviceProvider );
        }
        public ICommandHandler Create( Type handlerType ) 
        {
            return _s.CreateInstanceOrDefault<ICommandHandler>( handlerType );
        }
    }

}
