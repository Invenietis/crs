using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Tooling;
using System.Linq;

namespace Cake.Npm
{
    /// <summary>
    /// Get the npm version
    /// </summary>
    public class NpmGetNpmVersion : NpmSettings
    {
        public NpmGetNpmVersion() : base( "-v" )
        {
            RedirectStandardOutput = true;
        }

        public string PackageName { get; set; }

        protected override void EvaluateCore( ProcessArgumentBuilder args )
        {
        }
    }

    public class NpmGetNpmVersionTools : NpmTool<NpmSettings>
    {
        public NpmGetNpmVersionTools(
            IFileSystem fileSystem,
            ICakeEnvironment environment,
            IProcessRunner processRunner,
            IToolLocator tools,
            ICakeLog log )
            : base( fileSystem, environment, processRunner, tools, log )
        {
        }

        public string GetNpmVersion()
        {
            try
            {
                string output = null;
                RunCore( new NpmGetNpmVersion() { }, new ProcessSettings(), process =>
                 {
                     output = process.GetStandardOutput().Single();
                 } );
                return output;

            }
            catch( CakeException ex )
            {
                CakeLog.Information( "Error while calling npm -v: " + ex.Message );
                return "";
            }

        }
    }

    [CakeAliasCategory( "Npm" )]
    [CakeNamespaceImport( "Cake.Npm" )]
    public static class NpmGetNpmVersionAliases
    {
        /// <summary>
        /// Call npm view with --json attribute.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="packageName">Name of the package</param>
        /// <param name="workingDirectory"></param>
        /// <returns>An empty string if the package was not found on the repository</returns>
        [CakeMethodAlias]
        [CakeAliasCategory( "Get" )]
        public static string NpmGetNpmVersion( this ICakeContext context )
        {
            return new NpmGetNpmVersionTools( context.FileSystem, context.Environment, context.ProcessRunner, context.Tools, context.Log ).GetNpmVersion();
        }
    }

}
