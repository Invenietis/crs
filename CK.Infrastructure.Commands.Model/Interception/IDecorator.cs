using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface IDecorator
    {
        int Order { get; }

        void OnCommandExecuting( CommandExecutionContext ctx );

        void OnException( CommandExecutionContext ctx );

        void OnCommandExecuted( CommandExecutionContext ctx );
    }
}
