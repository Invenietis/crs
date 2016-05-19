using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public class ExternalComponents
    {
        /// <summary>
        /// Gets or sets a command scheduler for the current pipeline.
        /// </summary>
        public IOperationExecutor<ScheduledCommand> CommandScheduler { get; set; }

        /// <summary>
        /// Gets or sets an event publisher for the current pipeline.
        /// </summary>
        public IOperationExecutor<Event> EventPublisher { get; set; }

        public ICommandResponseDispatcher ResponseDispatcher { get; set; }
    }
}
