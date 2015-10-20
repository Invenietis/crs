using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandFactory
    {
        ICommand CreateCommand( CommandRouteRegistration routeInfo, Stream requestPayload );
    }
}
