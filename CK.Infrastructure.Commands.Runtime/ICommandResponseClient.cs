using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandResponseClient
    {
        Task ReceiveCommandResponse( ICommandResponse response );
    }

}
