using CSemVer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CodeCake
{
    /// <summary>
    /// Encapsulates a set of <see cref="NPMProject"/> that can be <see cref="NPMPublishedProject"/>.
    /// </summary>
    public class NPMSolution
    {
        /// <summary>
        /// Initiaizes a new <see cref="NPMSolution" />.
        /// </summary>
        /// <param name="projects">Set of projects.</param>
        public NPMSolution( IEnumerable<NPMProject> projects )
        {
            Projects = projects.ToArray();
            PublishedProjects = Projects.OfType<NPMPublishedProject>().ToArray();
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
        public void RunPack( bool cleanupPackageJson = true, Action<JObject> packageJsonPreProcessor = null )
        {
            foreach( var p in PublishedProjects )
            {
                p.RunPack( cleanupPackageJson, packageJsonPreProcessor );
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
            return new NPMSolution( projects );
        }
    }
}
