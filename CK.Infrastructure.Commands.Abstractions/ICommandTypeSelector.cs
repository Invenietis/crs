using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandTypeSelector
    {
        Type DetermineCommandType( ICommandReceiverOptions receiverOptions, CommandRoutePath routePath );
    }
}
