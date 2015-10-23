using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    /// <summary>
    /// Holds the assoication of a command a route.
    /// </summary>
    public class CommandRouteRegistration
    {
        public CommandRoutePath Route { get; set; }

        public Type CommandType { get; set; }

        public bool IsLongRunningCommand { get; set; }
    }
}
