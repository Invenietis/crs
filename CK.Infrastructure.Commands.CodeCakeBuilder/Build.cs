using Cake.Core;
using Code.Cake;
using SimpleGitVersion;
using System.Collections.Generic;
using System;
using System.Linq;
using Cake.Common.Diagnostics;
using Cake.Common.IO;

namespace CodeCake
{
    /// <summary>
    /// Sample build "script".
    /// It can be decorated with AddPath attributes that inject paths into the PATH environment variable. 
    /// </summary>
    [AddPath( "CodeCakeBuilder/Tools" )]
    public class Build : CodeCakeHost
    {
        public Build()
        {
            DNXSolution dnxSolution = null;
            IEnumerable<DNXProjectFile> projectsToPublish = null;
            SimpleRepositoryInfo gitInfo = null;
            string configuration = null;

            Setup( () =>
            {
                dnxSolution = Cake.GetDNXSolution( p => p.ProjectName != "CodeCakeBuilder" );
                if( !dnxSolution.IsValid ) throw new Exception( "Unable to initialize solution." );
                projectsToPublish = dnxSolution.Projects.Where( p => (!p.ProjectName.EndsWith( ".Tests" ) || !p.ProjectName.EndsWith( "Integration" )) );
            } );

            Teardown( () =>
            {
                dnxSolution.RestoreProjectFiles();
            } );

            Task( "Check-Repository" )
                .Does( () =>
                {
                    gitInfo = dnxSolution.RepositoryInfo;
                    if( !gitInfo.IsValid )
                    {
                        if( Cake.IsInteractiveMode()
                            && Cake.ReadInteractiveOption( "Repository is not ready to be published. Proceed anyway?", 'Y', 'N' ) == 'Y' )
                        {
                            Cake.Warning( "GitInfo is not valid, but you choose to continue..." );
                        }
                        else throw new Exception( "Repository is not ready to be published." );
                    }
                    configuration = gitInfo.IsValidRelease && gitInfo.PreReleaseName.Length == 0 ? "Release" : "Debug";
                    Cake.Information( "Publishing {0} projects with version={1} and configuration={2}: {3}",
                        projectsToPublish.Count(),
                        gitInfo.SemVer,
                        configuration,
                        String.Join( ", ", projectsToPublish.Select( p => p.ProjectName ) ) );
                } );

            Task( "Set-ProjectVersion" )
                .IsDependentOn( "Check-Repository" )
                .Does( () =>
                {
                    if( dnxSolution.UpdateProjectFiles( true ) > 0 )
                    {
                        Cake.DNURestore( c =>
                        {
                            c.Quiet = true;
                            c.ProjectPaths.UnionWith( dnxSolution.Projects.Select( p => p.ProjectFilePath ) );
                        } );
                    }
                } );

            Task( "Clean" )
                .IsDependentOn( "Check-Repository" )
                .Does( () =>
                {
                    Cake.CleanDirectories( "**/bin/" + configuration, d => !d.Path.Segments.Contains( "CodeCakeBuilder" ) );
                    Cake.CleanDirectories( "**/obj/" + configuration, d => !d.Path.Segments.Contains( "CodeCakeBuilder" ) );
                    Cake.DeleteFiles( "Tests/**/TestResult.xml" );
                } );

            Task( "Build-And-Pack" )
              .IsDependentOn( "Clean" )
              .IsDependentOn( "Set-ProjectVersion" )
              .Does( () =>
              {
                  Cake.DNUBuild( c =>
                  {
                      c.GeneratePackage = true;
                      c.Configurations.Add( configuration );
                      c.ProjectPaths.UnionWith( projectsToPublish.Select( p => p.ProjectDir ) );
                      c.Quiet = true;
                  } );
              } );

            Task( "Push-NuGet-Packages-Packarium" )
                .IsDependentOn( "Build-And-Pack" )
                .Does( () =>
                {
                    var allPackages = Cake.GetFiles( "**/bin/" + configuration + "/*.nupkg" ).Select( f => f.FullPath ).ToList();
                    var nugetPackages = allPackages.Where( fileName => !fileName.EndsWith( ".symbols.nupkg" ) ).ToList();
                    var packagesSymbols = allPackages.Where( fileName => fileName.EndsWith( ".symbols.nupkg" ) ).ToList();

                    Cake.Information( "Found {0} packages (and {1} symbols) to push in {2}.", nugetPackages.Count, packagesSymbols.Count, configuration );
                    if( gitInfo.IsValid )
                    {
                        if( Cake.IsInteractiveMode() )
                        {
                            var localFeed = Cake.FindDirectoryAbove( "Packarium" );
                            if( localFeed != null )
                            {
                                Cake.Information( "LocalFeed directory found: {0}", localFeed );
                                if( Cake.ReadInteractiveOption( "Do you want to publish to LocalFeed?", 'Y', 'N' ) == 'Y' )
                                {
                                    Cake.CopyFiles( nugetPackages, localFeed );
                                    Cake.CopyFiles( packagesSymbols, localFeed );
                                }
                            }
                        }
                    }
                    else
                    {
                        Cake.Information( "Push-NuGet-Packages step is skipped since Git repository info is not valid." );
                    }
                } );
            // The Default task for this script can be set here.
            Task( "Default" )
                .IsDependentOn( "Push-NuGet-Packages-Packarium" );
        }
    }
}