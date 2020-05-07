using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Solution;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Common.Tools.NUnit;
using Cake.Core;
using Cake.Core.IO;
using CK.Text;
using CodeCake.Abstractions;
using SimpleGitVersion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CodeCake
{

    public static class StandardGlobalInfoDotnetExtension
    {
        public static StandardGlobalInfo AddDotnet( this StandardGlobalInfo globalInfo )
        {
            DotnetSolution sln = DotnetSolution.FromSolutionInCurrentWorkingDirectory( globalInfo );
            globalInfo.RegisterSolution( sln );
            return globalInfo;
        }

        public static DotnetSolution GetDotnetSolution( this StandardGlobalInfo globalInfo )
        {
            return globalInfo.Solutions.OfType<DotnetSolution>().Single();
        }
    }

    public partial class DotnetSolution : ICIWorkflow
    {
        readonly StandardGlobalInfo _globalInfo;
        public readonly string SolutionFileName;
        public readonly IReadOnlyList<SolutionProject> Projects;
        public readonly IReadOnlyList<SolutionProject> ProjectsToPublish;

        DotnetSolution(
            StandardGlobalInfo globalInfo,
            string solutionFileName,
            IReadOnlyList<SolutionProject> projects,
            IReadOnlyList<SolutionProject> projectsToPublish )
        {
            _globalInfo = globalInfo;
            SolutionFileName = solutionFileName;
            Projects = projects;
            ProjectsToPublish = projectsToPublish;
        }

        /// <summary>
        /// Create a DotnetSolution from the sln.
        /// </summary>
        /// <param name="globalInfo">The StandardGlobalInfo</param>
        /// <param name="projectToPublishPredicate">
        /// The predicate used to filter the project to publish. By default the predicate is p => !p.Path.Segments.Contains
        /// </param>
        /// <returns></returns>
        public static DotnetSolution FromSolutionInCurrentWorkingDirectory(
            StandardGlobalInfo globalInfo )
        {
            string solutionFileName = System.IO.Path.GetFileName(
                globalInfo.Cake.GetFiles( "*.sln",
                    new GlobberSettings
                    {
                        Predicate = p => !System.IO.Path.GetFileName( p.Path.FullPath ).EndsWith( ".local.sln", StringComparison.OrdinalIgnoreCase )
                    } ).Single().FullPath
            );
            var sln = globalInfo.Cake.ParseSolution( solutionFileName );

            var projects = sln
                .Projects
                .Where( p => !(p is SolutionFolder)
                            && p.Name != "CodeCakeBuilder" )
                .ToList();
            var projectsToPublish = projects.Where(
                    p => ((bool?)XDocument.Load( p.Path.FullPath )
                        .Root
                        .Elements( "PropertyGroup" )
                        .Elements( "IsPackable" ).LastOrDefault() ?? true) == true
                )
                .ToList();

            return new DotnetSolution( globalInfo, solutionFileName, projects, projectsToPublish );
        }

        /// <summary>
        /// Cleans the bin/obj of the projects and the TestResults xml.
        /// </summary>
        public void Clean()
        {
            _globalInfo.Cake.CleanDirectories( Projects.Select( p => p.Path.GetDirectory().Combine( "bin" ) ) );
            _globalInfo.Cake.CleanDirectories( Projects.Select( p => p.Path.GetDirectory().Combine( "obj" ) ) );
            _globalInfo.Cake.DeleteFiles( "Tests/**/TestResult*.xml" );
        }

        /// <summary>
        /// Builds the solution without "CodeCakeBuilder" project itself and
        /// optionally other projects.
        /// </summary>
        /// <param name="excludedProjectName">Optional project names (without path nor .csproj extension).</param>
        public void Build( params string[] excludedProjectsName )
        {
            using( var tempSln = _globalInfo.Cake.CreateTemporarySolutionFile( SolutionFileName ) )
            {
                var exclude = new List<string>( excludedProjectsName ) { "CodeCakeBuilder" };
                tempSln.ExcludeProjectsFromBuild( exclude.ToArray() );
                _globalInfo.Cake.DotNetCoreBuild( tempSln.FullPath.FullPath,
                    new DotNetCoreBuildSettings().AddVersionArguments( _globalInfo.BuildInfo, s =>
                    {
                        s.Configuration = _globalInfo.BuildInfo.BuildConfiguration;
                    } ) );
            }
        }

        public void Test( IEnumerable<SolutionProject> testProjects = null )
        {
            if( testProjects == null )
            {
                testProjects = Projects.Where( p => p.Name.EndsWith( ".Tests" ) );
            }

            foreach( SolutionProject project in testProjects )
            {
                NormalizedPath projectPath = project.Path.GetDirectory().FullPath;
                NormalizedPath binDir = projectPath.AppendPart( "bin" ).AppendPart( _globalInfo.BuildInfo.BuildConfiguration );
                NormalizedPath objDir = projectPath.AppendPart( "obj" );
                string assetsJson = File.ReadAllText( objDir.AppendPart( "project.assets.json" ) );
                bool isNunitLite = assetsJson.Contains( "NUnitLite" );
                bool isVSTest = assetsJson.Contains( "Microsoft.NET.Test.Sdk" );
                foreach( NormalizedPath buildDir in
                    Directory.GetDirectories( binDir )
                        .Where( p => Directory.EnumerateFiles( p ).Any() )
                )
                {
                    string framework = buildDir.LastPart;
                    bool isNetFramework = framework.StartsWith( "net" ) && framework.Length == 6 && int.TryParse( framework.Substring( 3 ), out var _ );
                    string fileWithoutExtension = buildDir.AppendPart( project.Name );
                    string testBinariesPath = "";
                    if( isNunitLite )
                    {
                        // Using NUnitLite.
                        if( isNetFramework && File.Exists( (testBinariesPath = fileWithoutExtension + ".exe") ) )
                        {
                            _globalInfo.Cake.Information( $"Testing via NUnitLite ({framework}): {testBinariesPath}" );
                            if( !_globalInfo.CheckCommitMemoryKey( testBinariesPath ) )
                            {
                                _globalInfo.Cake.NUnit3( new[] { testBinariesPath }, new NUnit3Settings
                                {
                                    Results = new[] { new NUnit3Result() { FileName = FilePath.FromString( projectPath.AppendPart( "TestResult.Net461.xml" ) ) } }
                                } );
                            }
                        }
                        else
                        {
                            testBinariesPath = fileWithoutExtension + ".dll";
                            _globalInfo.Cake.Information( $"Testing via NUnitLite ({framework}): {testBinariesPath}" );
                            if( !_globalInfo.CheckCommitMemoryKey( testBinariesPath ) )
                            {
                                _globalInfo.Cake.DotNetCoreExecute( testBinariesPath );
                            }
                        }
                    }
                    if( isVSTest )
                    {
                        testBinariesPath = fileWithoutExtension + ".dll";
                        // VS Tests
                        _globalInfo.Cake.Information( $"Testing via VSTest ({framework}): {testBinariesPath}" );
                        if( !_globalInfo.CheckCommitMemoryKey( testBinariesPath ) )
                        {
                            var options = new DotNetCoreTestSettings()
                            {
                                Configuration = _globalInfo.BuildInfo.BuildConfiguration,
                                Framework = framework,
                                NoRestore = true,
                                NoBuild = true,
                                Logger = "trx"
                            };
                            if( _globalInfo.Cake.Environment.GetEnvironmentVariable( "DisableNodeReUse" ) != null )
                            {
                                options.ArgumentCustomization = args => args.Append( "/nodeReuse:false" );
                            }
                            _globalInfo.Cake.DotNetCoreTest( projectPath, options );
                        }
                    }

                    if( !isVSTest && !isNunitLite )
                    {
                        testBinariesPath = fileWithoutExtension + ".dll";
                        _globalInfo.Cake.Information( "Testing via NUnit: {0}", testBinariesPath );
                        if( !_globalInfo.CheckCommitMemoryKey( testBinariesPath ) )
                        {
                            _globalInfo.Cake.NUnit( new[] { testBinariesPath }, new NUnitSettings()
                            {
                                Framework = "v4.5",
                                ResultsFile = FilePath.FromString( projectPath.AppendPart( "TestResult.Net461.xml" ) )
                            } );

                        }
                    }
                    _globalInfo.WriteCommitMemoryKey( testBinariesPath );
                }
            }
        }

        void ICIWorkflow.Build() => Build();

        void ICIWorkflow.Test() => Test();
    }
}
