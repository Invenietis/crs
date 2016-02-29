using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{
    public class ScheduledCommand
    {
        public Guid CommandId { get; set; }
        /// <summary>
        /// Gets the correlation identifier.
        /// </summary>
        public string CorrelationId { get; set; }
        /// <summary>
        /// Gets the callback identifier where response of this scheduled command should be routed.
        /// </summary>
        public string CallbackId { get; set; }
        /// <summary>
        /// Gets or sets the scheduling configuration for this command.
        /// </summary>
        public CommandSchedulingOption Scheduling { get; set; }
        /// <summary>
        /// Gets 
        /// </summary>
        public RoutedCommandDescriptor Description { get; set; }
        public ActivityMonitor.DependentToken Token { get; set; }

        public object Payload { get; set; }
    }

}
