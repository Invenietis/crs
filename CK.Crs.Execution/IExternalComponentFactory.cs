using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Execution
{
    public interface IExternalComponentFactory
    {
        /// <summary>
        /// Creates a <see cref="ICommandResponseDispatcher"/> for the current execution context.
        /// </summary>
        /// <returns></returns>
        ICommandResponseDispatcher CreateResponseDispatcher();

        /// <summary>
        /// Creates an instance of <see cref="IOperationExecutor&lt;Event&gt;"/> used by an <see cref="IExternalEventPublisher"/> to publish events to the external world.
        /// </summary>
        /// <returns></returns>
        IOperationExecutor<Event> CreateExternalEventPublisher();

        /// <summary>
        /// Creates an instance of <see cref="IOperationExecutor&lt;ScheduledCommand&gt;"/> that will be used by the <see cref="ICommandScheduler"/> to schedule commands.
        /// </summary>
        /// <returns></returns>
        IOperationExecutor<ScheduledCommand> CreateCommandScheduler();
    }
}
