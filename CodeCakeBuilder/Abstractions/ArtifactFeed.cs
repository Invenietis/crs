using Cake.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeCake.Abstractions
{
    /// <summary>
    /// Abstract feed associated to a <see cref="ArtifactType"/>.
    /// </summary>
    public abstract class ArtifactFeed
    {
        protected ArtifactFeed( ArtifactType t )
        {
            ArtifactType = t;
        }

        /// <summary>
        /// Gets the artifact type that handles this feed.
        /// </summary>
        public ArtifactType ArtifactType { get; }

        /// <summary>
        /// Gets the cake context.
        /// </summary>
        protected ICakeContext Cake => ArtifactType.GlobalInfo.Cake;

        /// <summary>
        /// Name used to print the feed in the logs.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Creates a list of <see cref="ArtifactPush"/> object from a set of <see cref="ILocalArtifact"/>.
        /// </summary>
        /// <param name="artifacts">Set of artifacts.</param>
        /// <returns>The set of push information.</returns>
        public abstract Task<IEnumerable<ArtifactPush>> CreatePushListAsync( IEnumerable<ILocalArtifact> artifacts );

        /// <summary>
        /// Pushes a set of artifacts previously computed by <see cref="CreatePushListAsync(IEnumerable{ILocalArtifact})"/>.
        /// </summary>
        /// <param name="pushes">The instances to push (that necessary target this feed).</param>
        /// <returns>The awaitable.</returns>
        public abstract Task PushAsync( IEnumerable<ArtifactPush> pushes );

    }
}
