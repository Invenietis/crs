using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandHandler
    {
        Task<object> HandleAsync( object command );
    }

    public interface ICommandHandler<in T>
    {
        Task<object> HandleAsync( T command );
    }
}
