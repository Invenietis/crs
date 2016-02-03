using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    /// <summary>
    /// Describes a command and its environment.
    /// </summary>
    public class CommandDescriptor
    {
        public CommandDescriptor()
        {
            Decorators = CK.Core.Util.EmptyArray<Type>.Empty;
        }

        /// <summary>
        /// The name of the command
        /// </summary>
        public string Name { get; set; }

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
        public IReadOnlyCollection<Type> Decorators { get; set; }

        public IReadOnlyCollection<ICommandExecutor> Executors { get; set; }
    }
}
