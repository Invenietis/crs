using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Infrastructure.Commands;
using CK.Infrastructure.Commands;

namespace CK.Infrastructure.Commands
{
    public class ConventionCommandTypeSelector : ICommandTypeSelector
    {
        public Type DetermineCommandType( ICommandReceiverOptions receiverOptions, string requestPath )
        {
            return typeof( CommandBase );
        }
    }
}
