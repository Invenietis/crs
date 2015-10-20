using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandRouteMap
    {
        CommandRouteRegistration FindCommandRoute( string requestPath, ICommandReceiverOptions options );

        ICommandRouteMap Register( CommandRouteRegistration route );
    }

    public class CommandRouteRegistration
    {
        public CommandRoutePath Route { get; set; }

        public Type CommandType { get; set; }
    }
}
