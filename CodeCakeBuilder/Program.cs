using System;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCake
{
    class Program
    {
        /// <summary>
        /// Basic parameter that sets the solution directory as being the current directory
        /// instead of using the default lookup to "Solution/Builder/bin/[Configuration]/[targetFramework]" folder.
        /// Check of this argument uses <see cref="StringComparer.OrdinalIgnoreCase"/>.
        /// </summary>
        const string SolutionDirectoryIsCurrentDirectoryParameter = "SolutionDirectoryIsCurrentDirectory";

        /// <summary>
        /// CodeCakeBuilder entry point. This is a default, simple, implementation that can 
        /// be extended as needed.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>An error code (typically negative), 0 on success.</returns>
        static async Task<int> Main( string[] args )
        {
            string? solutionDirectory = args.Contains( SolutionDirectoryIsCurrentDirectoryParameter, StringComparer.OrdinalIgnoreCase )
                                        ? Environment.CurrentDirectory
                                        : null;
            var app = new CodeCakeApplication( solutionDirectory );
            RunResult result = await app.RunAsync( args.Where( a => !StringComparer.OrdinalIgnoreCase.Equals( a, SolutionDirectoryIsCurrentDirectoryParameter ) ) );
            return result.ReturnCode;
        }
    }
}
