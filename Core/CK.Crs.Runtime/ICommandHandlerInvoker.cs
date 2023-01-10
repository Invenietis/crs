using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface ICommandHandlerInvoker
    {
        Task<object> Invoke( object command, ICommandContext context );

    }
}
