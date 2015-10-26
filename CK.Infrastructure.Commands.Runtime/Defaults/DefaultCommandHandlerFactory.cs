using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandHandlerFactory : ICommandHandlerFactory
    {
        public ICommandHandler Create( Type handlerType ) 
        {
            return Activator.CreateInstance( handlerType ) as ICommandHandler;
        }
    }

}
