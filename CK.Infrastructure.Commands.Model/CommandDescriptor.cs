using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    /// <summary>
    /// Holds the assoication of a command a route.
    /// </summary>
    public class CommandDescriptor
    {
        public CommandDescriptor()
        {
            Decorators = Enumerable.Empty<ICommandDecorator>();
        }

        /// <summary>
        /// Gets or sets the <see cref="CommandRoutePath"/> this command should be handled.
        /// </summary>
        public CommandRoutePath Route { get; set; }

        /// <summary>
        /// The <see cref="Type"/> of the command to process.
        /// </summary>
        public Type CommandType { get; set; }

        /// <summary>
        /// The <see cref="Type"/> of the command to process.
        /// </summary>
        public Type HandlerType { get; set; }

        /// <summary>
        /// Gets wether the command has been defined as a long running or not.
        /// This gives a hint to the <see cref="ICommandReceiver"/>.
        /// </summary>
        public bool IsLongRunning { get; set; }

        /// <summary>
        /// Gets or sets a read-only collection of decorators that should be applied when this command is handled.
        /// </summary>
        public IEnumerable<ICommandDecorator> Decorators { get; set; }
    }
}
