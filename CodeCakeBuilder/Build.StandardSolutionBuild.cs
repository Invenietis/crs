using Cake.Common.Solution;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using SimpleGitVersion;
using System.Collections.Generic;

namespace CodeCake
{
    public partial class Build
    {
        /// <summary>
        /// Builds the provided .sln without "CodeCakeBuilder" project itself and
        /// optionally other projects.
        /// </summary>
        /// <param name="solutionFileName">The solution file name to build (relative to the repository root).</param>
        /// <param name="gitInfo">The current git info.</param>
        /// <param name="configuration">The build configuration.</param>
        /// <param name="excludedProjectName">Optional project names (without path nor .csproj extension).</param>
        void StandardSolutionBuild( string solutionFileName, SimpleRepositoryInfo gitInfo, string configuration, params string[] excludedProjectName )
        {
            using( var tempSln = Cake.CreateTemporarySolutionFile( solutionFileName ) )
            {
                var exclude = new List<string>( excludedProjectName );
                exclude.Add( "CodeCakeBuilder" );
                tempSln.ExcludeProjectsFromBuild( exclude.ToArray() );
                Cake.DotNetCoreBuild( tempSln.FullPath.FullPath,
                    new DotNetCoreBuildSettings().AddVersionArguments( gitInfo, s =>
                    {
                        s.Configuration = configuration;
                    } ) );
            }
        }

    }
}
