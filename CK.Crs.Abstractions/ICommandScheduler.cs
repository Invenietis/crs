using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public interface ICommandScheduler
    {
        /// <summary>
        /// Schedules the execution of a command and returns an identifier of it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="monitor">The <see cref="IActivityMonitor"/ ></param>
        /// <param name="option">Configures the scheduling of the command</param>
        /// <param name="command"></param>
        /// <returns></returns>
        Guid Schedule<T>( IActivityMonitor monitor, T command, CommandSchedulingOption option );

        /// <summary>
        /// Cancel the scheduling of the given command identifier.
        /// If it fails this will returns false but no exception should be thrown.
        /// </summary>
        /// <param name="monitor"></param>
        /// <param name="commandId">The command identifier that targets the command to cancel.</param>
        /// <returns></returns>
        bool CancelScheduling( IActivityMonitor monitor, Guid commandId );
    }

    public class CommandSchedulingOption
    {
        public CommandSchedulingOption( DateTime time, bool cancellable = true )
        {
            WhenCommandShouldBeRaised = time;
            Cancellable = cancellable;
        }

        public CommandSchedulingOption( TimeSpan laps, bool cancellable = true )
            : this( DateTime.UtcNow.Add( laps ), cancellable )
        {
        }

        public DateTime WhenCommandShouldBeRaised { get; }
        public bool Cancellable { get; }
    }
}
