using Cake.Npm;
using CodeCake.Abstractions;
using CSemVer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CodeCake
{
    public static class StandardGlobalInfoNPMExtension
    {
        /// <summary>
        /// Adds the <see cref="NPMSolution"/> to the <paramref name="globalInfo"/>
        /// </summary>
        /// <param name="this">This global info.</param>
        /// <param name="solution">The NPM solution.</param>
        /// <returns>This info.</returns>
        public static StandardGlobalInfo AddNPM( this StandardGlobalInfo globalInfo, NPMSolution solution )
        {
            SVersion minmimalNpmVersionRequired = SVersion.Create( 6, 7, 0 );
            string npmVersion = globalInfo.Cake.NpmGetNpmVersion();
            if( SVersion.Parse( npmVersion ) < minmimalNpmVersionRequired )
            {
                globalInfo.Cake.TerminateWithError( "Outdated npm. Version older than v6.7.0 are known to fail on publish." );
            }
            globalInfo.RegisterSolution( solution );
            return globalInfo;
        }

        /// <summary>
        /// Adds the <see cref="Build.NPMArtifactType"/> for NPM based on <see cref="NPMSolution.ReadFromNPMSolutionFile"/>
        /// (projects are defined by "CodeCakeBuilder/NPMSolution.xml" file).
        /// </summary>
        /// <param name="this">This global info.</param>
        /// <returns>This info.</returns>
        public static StandardGlobalInfo AddNPM( this StandardGlobalInfo @this )
        {
            return AddNPM( @this, NPMSolution.ReadFromNPMSolutionFile( @this ) );
        }

        /// <summary>
        /// Gets the NPM solution handled by the single <see cref="Build.NPMArtifactType"/>.
        /// </summary>
        /// <param name="this">This global info.</param>
        /// <returns>The NPM solution.</returns>
        public static NPMSolution GetNPMSolution( this StandardGlobalInfo @this )
        {
            return @this.Solutions.OfType<NPMSolution>().Single();
        }

    }

    /// <summary>
    /// Encapsulates a set of <see cref="NPMProject"/> that can be <see cref="NPMPublishedProject"/>.
    /// </summary>
    public partial class NPMSolution : ISolution
    {
        readonly StandardGlobalInfo _globalInfo;

        /// <summary>
        /// Initiaizes a new <see cref="NPMSolution" />.
        /// </summary>
        /// <param name="projects">Set of projects.</param>
        NPMSolution( StandardGlobalInfo globalInfo, IEnumerable<NPMProject> projects )
        {
            Projects = projects.ToArray();
            PublishedProjects = Projects.OfType<NPMPublishedProject>().ToArray();
            _globalInfo = globalInfo;
        }

        /// <summary>
        /// Gets all the NPM projects.
        /// </summary>
        public IReadOnlyList<NPMProject> Projects { get; }

        /// <summary>
        /// Gets the published projects.
        /// </summary>
        public IReadOnlyList<NPMPublishedProject> PublishedProjects { get; }

        /// <summary>
        /// Runs "npm install" on all <see cref="Projects"/>.
        /// </summary>
        /// <param name="globalInfo">The global information object.</param>
        public void RunInstall()
        {
            foreach( var p in Projects )
            {
                p.RunInstall();
            }
        }

        /// <summary>
        /// Runs "npm install"  and a required (or optional) "clean" script on all <see cref="Projects"/>.
        /// </summary>
        /// <param name="scriptMustExist">
        /// False to only emit a warning and return false if the script doesn't exist instead of
        /// throwing an exception.
        /// </param>
        /// <param name="cleanScriptName">The script name that must exist in the package.json.</param>
        public void RunInstallAndClean( bool scriptMustExist = true, string cleanScriptName = "clean" )
        {
            foreach( var p in Projects )
            {
                p.RunInstallAndClean( scriptMustExist, cleanScriptName );
            }
        }

        /// <summary>
        /// Runs the 'build-debug', 'build-release' or 'build' script on all <see cref="Projects"/>.
        /// </summary>
        /// <param name="globalInfo">The global information object.</param>
        /// <param name="scriptMustExist">
        /// False to only emit a warning and return false if the script doesn't exist instead of
        /// throwing an exception.
        /// </param>
        public void RunBuild( bool scriptMustExist = true )
        {
            foreach( var p in Projects )
            {
                p.RunBuild( scriptMustExist );
            }
        }

        /// <summary>
        /// Runs the 'test' script on all <see cref="Projects"/>.
        /// </summary>
        /// <param name="globalInfo">The global information object.</param>
        /// <param name="globalInfo"></param>
        /// <param name="scriptMustExist">
        /// False to only emit a warning and return false if the script doesn't exist instead of
        /// throwing an exception.
        /// </param>
        public void RunTest( bool scriptMustExist = true )
        {
            foreach( var p in Projects )
            {
                p.RunTest( scriptMustExist );
            }
        }

        /// <summary>
        /// Generates the .tgz file in the <see cref="StandardGlobalInfo.ReleasesFolder"/>
        /// by calling npm pack for all <see cref="PublishedProjects"/>.
        /// </summary>
        /// <param name="globalInfo">The global information object.</param>
        /// <param name="cleanupPackageJson">
        /// By default, "scripts" and "devDependencies" are removed from the package.json file.
        /// </param>
        /// <param name="packageJsonPreProcessor">Optional package.json pre processor.</param>
        public void RunPack( Action<JObject> packageJsonPreProcessor = null )
        {
            foreach( var p in PublishedProjects )
            {
                p.RunPack( packageJsonPreProcessor );
            }
        }

        /// <summary>
        /// Reads the "CodeCakeBuilder/NPMSolution.xml" file that must exist.
        /// </summary>
        /// <param name="version">The version of all published packages.</param>
        /// <returns>The solution object.</returns>
        public static NPMSolution ReadFromNPMSolutionFile( StandardGlobalInfo globalInfo )
        {
            var projects = XDocument.Load( "CodeCakeBuilder/NPMSolution.xml" ).Root
                            .Elements( "Project" )
                            .Select( p => (bool)p.Attribute( "IsPublished" )
                                            ? NPMPublishedProject.Load( globalInfo, (string)p.Attribute( "Path" ),
                                                                        (string)p.Attribute( "ExpectedName" ),
                                                                        globalInfo.Version )
                                            : new NPMProject( globalInfo, (string)p.Attribute( "Path" ) ) );
            return new NPMSolution( globalInfo, projects );
        }

        void ISolution.Clean() => RunInstallAndClean( true );

        void ISolution.Build() => RunBuild( true );

        void ISolution.Test() => RunTest( true );
    }
}
