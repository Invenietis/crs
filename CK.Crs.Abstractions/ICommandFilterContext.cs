using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public interface ICommandFilterContext
    {
        /// <summary>
        /// Gets the <see cref="IActivityMonitor"/> for the current command processing
        /// </summary>
        IActivityMonitor Monitor { get; }

        /// <summary>
        /// Gets the command description.
        /// </summary>
        RoutedCommandDescriptor Description { get; }

        /// <summary>
        /// Gets the command instance
        /// </summary>
        object Command { get; }

        /// <summary>
        /// Rejects the command before execution.
        /// </summary>
        /// <param name="reason"></param>
        void Reject( string reason );

        /// <summary>
        /// Gets the principal which wants to execute this command.
        /// </summary>
        ClaimsPrincipal User { get; }

        IAmbientValues AmbientValues { get; }
    }
}
