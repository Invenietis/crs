using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Infrastructure.Commands
{
    public class CommandHandlerFactory : ICommandHandlerFactory
    {
        readonly IServiceProvider _serviceProvider;
        public CommandHandlerFactory( IServiceProvider serviceProvider )
        {
            _serviceProvider = serviceProvider;
        }
        public ICommandHandler Create( Type handlerType )
        {
            ICommandHandler handler = ActivatorUtilities.CreateInstance( _serviceProvider, handlerType ) as ICommandHandler;
            if( handler == null )
            {
                string msg = "Creation of the handler of type {0} failed. Make sure this type supports ICommandHandler interface.";
                throw new ArgumentException( msg );
            }

            return handler;
        }
    }
}
