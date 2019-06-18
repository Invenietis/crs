using Cake.Common.IO;
using Cake.Common.Solution;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.NUnit;
using Cake.Core;
using Cake.Core.Diagnostics;
using SimpleGitVersion;
using System;
using System.Linq;

namespace CodeCake
{
    /// <summary>
    /// Standard build "script".
    /// </summary>
    [AddPath( "%UserProfile%/.nuget/packages/**/tools*" )]
    public partial class Build : CodeCakeHost
    {
        public Build()
        {
            Cake.Log.Verbosity = Verbosity.Diagnostic;

            var solutionFileName = Cake.Environment.WorkingDirectory.GetDirectoryName() + ".sln";

            var projects = Cake.ParseSolution( solutionFileName )
                                       .Projects
                                       .Where( p => !(p is SolutionFolder) && p.Name != "CodeCakeBuilder" );

            // We do not generate NuGet packages for /Tests projects for this solution.
            var projectsToPublish = projects
                                        .Where( p => !p.Path.Segments.Contains( "Tests" ) );

            SimpleRepositoryInfo gitInfo = Cake.GetSimpleRepositoryInfo();
            StandardGlobalInfo globalInfo = CreateStandardGlobalInfo( gitInfo )
                                                .AddNuGet( projectsToPublish )
                                                .AddNPM()
                                                .SetCIBuildTag();

            Task( "Check-Repository" )
                .Does( () =>
                {
                    globalInfo.TerminateIfShouldStop();
                } );

            Task( "Clean" )
                .IsDependentOn( "Check-Repository" )
                .Does( () =>
                 {
                     Cake.CleanDirectories( projects.Select( p => p.Path.GetDirectory().Combine( "bin" ) ) );
                     Cake.CleanDirectories( projects.Select( p => p.Path.GetDirectory().Combine( "obj" ) ) );
                     Cake.CleanDirectories( globalInfo.ReleasesFolder );
                     Cake.DeleteFiles( "Tests/**/TestResult*.xml" );
                     globalInfo.GetNPMSolution().RunInstallAndClean( globalInfo, scriptMustExist: false );
                 } );


            Task( "Build" )
                .IsDependentOn( "Check-Repository" )
                .IsDependentOn( "Clean" )
                .Does( () =>
                 {
                     StandardSolutionBuild( globalInfo, solutionFileName );
                     globalInfo.GetNPMSolution().RunBuild( globalInfo );
                 } );

            Task( "Unit-Testing" )
                .IsDependentOn( "Build" )
                .WithCriteria( () => Cake.InteractiveMode() == InteractiveMode.NoInteraction
                                     || Cake.ReadInteractiveOption( "RunUnitTests", "Run Unit Tests?", 'Y', 'N' ) == 'Y' )
               .Does( () =>
                {
                    var testProjects = projects.Where( p => p.Name.EndsWith( ".Tests" )
                                                            && !p.Path.Segments.Contains( "Integration" ) );
                    StandardUnitTests( globalInfo, testProjects );
                    globalInfo.GetNPMSolution().RunTest( globalInfo );
                } );

            Task( "Build-Integration-Projects" )
                .IsDependentOn( "Unit-Testing" )
                .Does( () =>
                {
                    // Use WebApp.Tests to generate the StObj assembly.
                    var webAppTests = projects.Single( p => p.Name == "WebApp.Tests" );
                    var configuration = globalInfo.IsRelease ? "Release" : "Debug";
                    var path = webAppTests.Path.GetDirectory().CombineWithFilePath( "bin/" + configuration + "/net461/WebApp.Tests.dll" );
                    Cake.NUnit( path.FullPath, new NUnitSettings() { Include = "GenerateStObjAssembly" } );

                    var webApp = projects.Single( p => p.Name == "WebApp" );
                    Cake.DotNetCoreBuild( webApp.Path.FullPath,
                         new DotNetCoreBuildSettings().AddVersionArguments( gitInfo, s =>
                         {
                             s.Configuration = configuration;
                         } ) );
                } );

            Task( "Integration-Testing" )
                .IsDependentOn( "Build-Integration-Projects" )
                .WithCriteria( () => Cake.InteractiveMode() == InteractiveMode.NoInteraction
                                     || Cake.ReadInteractiveOption( "Run integration tests?", 'N', 'Y' ) == 'Y' )
                .Does( () =>
                {
                    var testIntegrationProjects = projects
                                                    .Where( p => p.Name.EndsWith( ".Tests" )
                                                                 && p.Path.Segments.Contains( "Integration" ) );
                    StandardUnitTests( globalInfo, testIntegrationProjects );
                } );


            Task( "Create-Packages" )
                .WithCriteria( () => gitInfo.IsValid )
                .IsDependentOn( "Unit-Testing" )
                .IsDependentOn( "Integration-Testing" )
                .Does( () =>
                 {
                     StandardCreateNuGetPackages( globalInfo );
                     globalInfo.GetNPMSolution().RunPack( globalInfo );
                 } );

            Task( "Push-Packages" )
                .WithCriteria( () => gitInfo.IsValid )
                .IsDependentOn( "Create-Packages" )
                .Does( () =>
                 {
                     globalInfo.PushArtifacts();
                 } );

            // The Default task for this script can be set here.
            Task( "Default" )
                .IsDependentOn( "Push-Packages" );

        }


    }
}
