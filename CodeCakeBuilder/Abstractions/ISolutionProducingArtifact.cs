namespace CodeCake.Abstractions
{
    interface ISolutionProducingArtifact
    {
        /// <summary>
        /// Pack the solution: it produce the artifacts.
        /// </summary>
        void Pack();

        ArtifactType ArtifactType { get; }
    }
}
