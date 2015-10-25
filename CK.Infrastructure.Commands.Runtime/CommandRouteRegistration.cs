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

        /// <summary>
        /// The <see cref="Type"/> of the command to process.
        /// </summary>
        public Type CommandType { get; set; }

        /// <summary>
        /// Gets wether the command has been defined as a long running or not.
        /// This gives a hint to the <see cref="ICommandReceiver"/>.
        /// </summary>
        public bool IsLongRunning { get; set; }
    }
}
