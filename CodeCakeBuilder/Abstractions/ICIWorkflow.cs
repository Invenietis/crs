namespace CodeCake.Abstractions
{
    public interface ICIWorkflow
    {
        /// <summary>
        /// Try to clean the folder, for example by deleting bin & obj.
        /// </summary>
        void Clean();

        /// <summary>
        /// Build the solution.
        /// </summary>
        void Build();

        /// <summary>
        /// Run the unit tests of the solution.
        /// </summary>
        void Test();
    }
}
