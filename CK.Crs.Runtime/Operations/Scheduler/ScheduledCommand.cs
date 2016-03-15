using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{
    public class ScheduledCommand : CommandAction
    {
        public ScheduledCommand( Guid commandId ) : base( commandId )
        {
        }

        /// <summary>
        /// Gets or sets the scheduling configuration for this command.
        /// </summary>
        public CommandSchedulingOption Scheduling { get; set; }

        public ActivityMonitor.DependentToken Token { get; set; }
    }

}
