using CK.Core;
using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Receives a command.
    /// </summary>
    public interface ICommandReceiver
    {
        /// <summary>
        /// A friendly name used to identify this <see cref="ICommandReceiver"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets wether this <see cref="ICommandReceiver"/> accepts to receive the given <see cref="ICommandContext"/>.
        /// </summary>
        /// <param name="context">The context to receive</param>
        /// <returns>True if accepted, false otherwise.</returns>
        bool AcceptCommand( object command, ICommandContext context );

        /// <summary>
        /// Receives the command and returns the appropriate <see cref="Response"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command to send.</param>
        /// <param name="context">The related <see cref="ICommandContext"/>.</param>
        /// <returns></returns>
        Task<Response> ReceiveCommand( object command, ICommandContext context );
    }
}
