namespace CodeCake.Abstractions
{
    /// <summary>
    /// Defines a locally produced artifact.
    /// This exposes an instance (with the version) even if it is the exact same version for all
    /// the artifact inside a repository: exposing the version here MAY enable, if needed, a "local"
    /// version definition for an artifact that COULD differ from the repository's one.
    /// </summary>
    public interface ILocalArtifact
    {
        /// <summary>
        /// Gets the artifact instance that is produced. 
        /// </summary>
        ArtifactInstance ArtifactInstance { get; }
    }
}
