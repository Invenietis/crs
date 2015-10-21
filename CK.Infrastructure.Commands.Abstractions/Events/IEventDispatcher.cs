using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{

    public interface IEventDispatcher
    {
        Task DispatchAsync( string callbackId, ICommandResponse response );
    }

}
