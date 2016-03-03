using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    /// <summary>
    /// <see cref="ICommandFilter"/> is invoked during command receiving.
    /// 
    /// </summary>
    public interface ICommandFilter
    {
        int Order { get; }

        Task OnCommandReceived( ICommandFilterContext context );
    }

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
    }
}