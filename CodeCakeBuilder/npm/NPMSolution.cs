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
            return AddNPM( @this, NPMSolution.ReadFromNPMSolutionFile( @this, false ) );
        }

        /// <summary>
        /// Adds the <see cref="Build.NPMArtifactType"/> for NPM based on <see cref="NPMSolution.ReadFromNPMSolutionFile"/>
        /// (projects are defined by "CodeCakeBuilder/NPMSolution.xml" file) using Yarn instead of NPM.
        /// </summary>
        /// <param name="this">This global info.</param>
        /// <returns>This info.</returns>
        public static StandardGlobalInfo AddYarn( this StandardGlobalInfo @this )
        {
            return AddNPM( @this, NPMSolution.ReadFromNPMSolutionFile( @this, true ) );
        }

        /// <summary>
        /// Gets the single NPM solution.
        /// </summary>
        /// <param name="this">This global info.</param>
        /// <returns>The NPM solution.</returns>
        public static NPMSolution GetNPMSolution( this StandardGlobalInfo @this )
        {
            var s = @this.Solutions.OfType<NPMSolution>().Single();
            if( s.UseYarn ) throw new Exception( "Solution is using Yarn. Call GetYarnSolution() instead of GetNPMSolution()." );
            return s;
        }

        /// <summary>
        /// Gets the single NPM solution (that must be using Yarn).
        /// </summary>
        /// <param name="this">This global info.</param>
        /// <returns>The NPM solution.</returns>
        public static NPMSolution GetYarnSolution( this StandardGlobalInfo @this )
        {
            var s = @this.Solutions.OfType<NPMSolution>().Single();
            if( !s.UseYarn ) throw new Exception( "Solution is using NPM, not Yarn. Call GetNPMSolution() instead of GetNPMSolution()." );
            return s;
        }

    }

    /// <summary>
    /// Encapsulates a set of <see cref="NPMProject"/> that can be <see cref="NPMPublishedProject"/>.
    /// </summary>
    public partial class NPMSolution : NPMProjectContainer, ICIWorkflow
    {

        /// <summary>
        /// Initializes a new <see cref="NPMSolution" />.
        /// </summary>
        /// <param name="projects">Set of projects.</param>
        NPMSolution( StandardGlobalInfo globalInfo, bool useYarn )
            : base()
        {
            GlobalInfo = globalInfo;
        }

        /// <summary>
        /// Gets whether Yarn is used instead of npm.
        /// </summary>
        public bool UseYarn { get; }

        public IEnumerable<AngularWorkspace> AngularWorkspaces => Containers.OfType<AngularWorkspace>();

        public StandardGlobalInfo GlobalInfo { get; }

        public void RunNpmCI()
        {
            foreach( var p in SimpleProjects )
            {
                p.RunNpmCi();
            }

            foreach( var p in AngularWorkspaces )
            {
                p.WorkspaceProject.RunNpmCi();
            }
        }

        public void Clean()
        {
            RunNpmCI();

            foreach( var p in SimpleProjects )
            {
                p.RunClean();
            }

            foreach( var p in AngularWorkspaces )
            {
                p.WorkspaceProject.RunClean();
            }
        }



        /// <summary>
        /// Runs the 'build-debug', 'build-release' or 'build' script on all <see cref="SimpleProjects"/>.
        /// </summary>
        /// <param name="globalInfo">The global information object.</param>
        /// <param name="scriptMustExist">
        /// False to only emit a warning and return false if the script doesn't exist instead of
        /// throwing an exception.
        /// </param>
        public void Build()
        {
            foreach( var p in SimpleProjects )
            {
                p.RunBuild();
            }
            foreach( var p in AngularWorkspaces )
            {
                p.WorkspaceProject.RunBuild();
            }
        }

        /// <summary>
        /// Runs the 'test' script on all <see cref="SimpleProjects"/>.
        /// </summary>
        /// <param name="globalInfo">The global information object.</param>
        /// <param name="globalInfo"></param>
        /// <param name="scriptMustExist">
        /// False to only emit a warning and return false if the script doesn't exist instead of
        /// throwing an exception.
        /// </param>
        public void Test()
        {
            foreach( var p in SimpleProjects )
            {
                p.RunTest();
            }
            foreach( var p in AngularWorkspaces )
            {
                p.WorkspaceProject.RunTest();
            }
        }

        /// <summary>
        /// Generates the .tgz file in the <see cref="StandardGlobalInfo.ReleasesFolder"/>
        /// by calling npm pack for all <see cref="SimplePublishedProjects"/>.
        /// </summary>
        /// <param name="globalInfo">The global information object.</param>
        /// <param name="cleanupPackageJson">
        /// By default, "scripts" and "devDependencies" are removed from the package.json file.
        /// </param>
        /// <param name="packageJsonPreProcessor">Optional package.json pre processor.</param>
        public void RunPack( Action<JObject> packageJsonPreProcessor = null )
        {
            foreach( var p in AllPublishedProjects )
            {
                p.RunPack( packageJsonPreProcessor );
            }
        }

        /// <summary>
        /// Reads the "CodeCakeBuilder/NPMSolution.xml" file that must exist.
        /// </summary>
        /// <param name="globalInfo">The global information.</param>
        /// <param name="useYarn">True to use Yarn instead of NPM.</param>
        /// <returns>The solution object.</returns>
        public static NPMSolution ReadFromNPMSolutionFile( StandardGlobalInfo globalInfo, bool useYarn )
        {
            var document = XDocument.Load( "CodeCakeBuilder/NPMSolution.xml" ).Root;
            var solution = new NPMSolution( globalInfo, useYarn );
            foreach( var item in document.Elements( "AngularWorkspace" ) )
            {
                solution.Add( AngularWorkspace.Create( solution,
                                                       (string)item.Attribute( "Path" ) ) );
            }
            foreach( var item in document.Elements( "Project" ) )
            {
                solution.Add( NPMPublishedProject.Create( solution,
                                                          (string)item.Attribute( "Path" ),
                                                          (string)item.Attribute( "OutputFolder" ) ) );
            }
            return solution;
        }
    }
}
