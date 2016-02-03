using System.Collections.Generic;

namespace CK.Infrastructure.Commands
{
    /// <summary>
    /// A command request is procesed by the <see cref="ICommandReceiver"/>.
    /// This defines all it needs to correctly handle the command.
    /// </summary>
    public interface ICommandRequest
    {
        /// <summary>
        /// Returns an identifier that should help identifying the caller of this request.
        /// </summary>
        string CallbackId { get; }

        /// <summary>
        /// Command description
        /// </summary>
        RoutedCommandDescriptor CommandDescription { get; }

        /// <summary>
        /// The instance of the command to process. This should never be null.
        /// </summary>
        object Command { get; }

        IReadOnlyCollection<BlobRef> Files { get; }
    }

}
