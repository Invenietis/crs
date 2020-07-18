using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Tooling;
using System.Collections.Generic;
using System.Linq;

namespace Cake.Npm
{
    /// <summary>
    /// Almost no options are supported.
    /// </summary>
    public class NpmViewSettings : NpmSettings
    {
        public NpmViewSettings() : base( "view" )
        {
            RedirectStandardOutput = true;
        }

        public string PackageName { get; set; }

        protected override void EvaluateCore( ProcessArgumentBuilder args )
        {
            base.EvaluateCore( args );
            args.Append( "--json" );
            args.Append( PackageName );
        }
    }

    public class NpmViewTools : NpmTool<NpmSettings>
    {
        public NpmViewTools(
            IFileSystem fileSystem,
            ICakeEnvironment environment,
            IProcessRunner processRunner,
            IToolLocator tools,
            ICakeLog log )
            : base( fileSystem, environment, processRunner, tools, log )
        {
        }

        public string View( NpmViewSettings settings )
        {
            IEnumerable<string> output = new List<string>();
            try
            {
                RunCore( settings, new ProcessSettings(), process =>
                {
                    output = process.GetStandardOutput();
                } );
                return string.Join( "\n", output );

            }
            catch( CakeException ex )
            {
                CakeLog.Information( "Error while calling npm view: " + ex.Message );
                return "";
            }

        }
    }

    [CakeAliasCategory( "Npm" )]
    [CakeNamespaceImport( "Cake.Npm" )]
    public static class NpmViewAliases
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
        public static string NpmView( this ICakeContext context, string packageName = null, string workingDirectory = null )
        {
            NpmViewSettings settings = new NpmViewSettings()
            {
                LogLevel = NpmLogLevel.Info,
                PackageName = packageName,
                WorkingDirectory = workingDirectory
            };
            return new NpmViewTools( context.FileSystem, context.Environment, context.ProcessRunner, context.Tools, context.Log ).View( settings );
        }
    }

}
