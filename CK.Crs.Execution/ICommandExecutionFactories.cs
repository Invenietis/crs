using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Execution
{
    public interface ICommandExecutionFactories : ICommandDecoratorFactory, ICommandHandlerFactory, IExternalComponentFactory
    {
    }

}
