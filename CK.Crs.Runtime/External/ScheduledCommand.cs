using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime;

namespace CK.Crs
{
    public class ScheduledCommand : CommandAction
    {
        public ScheduledCommand( Guid commandId ) : base( commandId )
        {
        }

        public IPipeline Pipeline { get; set; }

        /// <summary>
        /// Gets or sets the scheduling configuration for this command.
        /// </summary>
        public CommandSchedulingOption Scheduling { get; set; }

        public ActivityMonitor.DependentToken Token { get; set; }
    }

}
