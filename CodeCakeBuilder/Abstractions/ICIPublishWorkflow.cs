namespace CodeCake.Abstractions
{
    interface ICIPublishWorkflow
    {
        /// <summary>
        /// Pack the solution: it produce the artifacts.
        /// </summary>
        void Pack();

        ArtifactType ArtifactType { get; }
    }
}
