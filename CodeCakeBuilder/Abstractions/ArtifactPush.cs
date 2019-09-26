using CSemVer;
using System.Collections.Generic;

namespace CodeCake.Abstractions
{
    /// <summary>
    /// Represents the required push of a <see cref="ILocalArtifact"/> into a <see cref="Feed"/>.
    /// This class may be specialized if Feed per Artifact data must be generated and circulate between
    /// the <see cref="ArtifactFeed.CreatePushListAsync(IEnumerable{ILocalArtifact})"/>
    /// and <see cref="ArtifactFeed.PushAsync(IEnumerable{ArtifactPush})"/>.
    /// </summary>
    public class ArtifactPush
    {
        /// <summary>
        /// Initializes a new <see cref="ArtifactPush"/> instance.
        /// </summary>
        /// <param name="local">The artifact.</param>
        /// <param name="feed">The target feed.</param>
        public ArtifactPush( ILocalArtifact local, ArtifactFeed feed )
        {
            LocalArtifact = local;
            Feed = feed;
        }

        /// <summary>
        /// Gets the artifact.
        /// </summary>
        public ILocalArtifact LocalArtifact { get; }

        /// <summary>
        /// Gets the artifact name.
        /// </summary>
        public string Name => LocalArtifact.ArtifactInstance.Artifact.Name;

        /// <summary>
        /// Gets the artifact's version.
        /// </summary>
        public SVersion Version => LocalArtifact.ArtifactInstance.Version;

        /// <summary>
        /// Gets the feed.
        /// </summary>
        public ArtifactFeed Feed { get; }
    }

}
