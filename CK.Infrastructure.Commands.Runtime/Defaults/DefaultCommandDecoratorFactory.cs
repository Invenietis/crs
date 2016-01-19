using System;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandDecoratorFactory : ICommandDecoratorFactory
    {
        readonly DefaultCreateInstanceStrategy _s;
        public DefaultCommandDecoratorFactory( IServiceProvider serviceProvider )
        {
            _s = new DefaultCreateInstanceStrategy( serviceProvider );
        }
        public ICommandDecorator Create( Type handlerType )
        {
            return _s.CreateInstanceOrDefault<ICommandDecorator>( handlerType );
        }
    }

}
