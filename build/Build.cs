using Cake.Common;
using Cake.Common.Solution;
using Cake.Common.IO;
using Cake.Common.Tools.MSBuild;
using Cake.Common.Tools.NuGet;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.DotNetCore.Pack;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Core;
using Cake.Common.Diagnostics;
using SimpleGitVersion;
using Cake.Common.Tools.NuGet.Pack;
using System;
using System.Linq;
using Cake.Core.Diagnostics;
using Cake.Common.Text;
using Cake.Common.Tools.NuGet.Push;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Cake.Common.Tools.NUnit;
using Cake.Core.IO;

namespace CodeCake
{
    /// <summary>
    /// Sample build "script".
    /// It can be decorated with AddPath attributes that inject paths into the PATH environment variable. 
    /// </summary>
    [AddPath( "build/Tools" )]
    public class Build : CodeCakeHost
    {
        public Build()
        {
            var coreBuildFile = Cake.File( "CoreBuild.proj" );
            var releasesDir = Cake.Directory( "Releases" );
            SimpleRepositoryInfo gitInfo = null;
            string configuration = null;
            var projectsToPublish = Cake.ParseSolution( "../CK-Crs.sln" )
                                        .Projects
                                        .Where( p => p.Name != "CodeCakeBuilder"
                                                    && p.Type == "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}"
                                                    && !p.Path.Segments.Contains( "Tests", StringComparer.OrdinalIgnoreCase )
                                                    && !p.Path.Segments.Contains( "Samples", StringComparer.OrdinalIgnoreCase ) ) 
                                        .ToList();

            Task( "Check-Repository" )
                .Does( () =>
                {
                    gitInfo = Cake.GetSimpleRepositoryInfo();
                    if( !gitInfo.IsValid )
                    {
                        configuration = "Debug";
                        Cake.Warning( "Repository is not ready to be published. Setting configuration to {0}.", configuration );
                    }
                    else
                    {
                        configuration = gitInfo.IsValidRelease && gitInfo.PreReleaseName.Length == 0 ? "Release" : "Debug";
                    }
                    Cake.Information( "Publishing {0} projects with version={1} and configuration={2}: {3}",
                        projectsToPublish.Count(),
                        gitInfo.SafeSemVersion,
                        configuration,
                        string.Join( ", ", projectsToPublish.Select( p => p.Name ) ) );
                } );

            Task( "Restore-NuGet-Packages" )
                .IsDependentOn( "Check-Repository" )
                .Does( () =>
                 {
                     Cake.DotNetCoreRestore( coreBuildFile,
                         new DotNetCoreRestoreSettings().AddVersionArguments( gitInfo, c =>
                         {
                             // No impact see: https://github.com/NuGet/Home/issues/3772
                             // c.Verbosity = DotNetCoreRestoreVerbosity.Minimal;
                         } ) );
                 } );
            Task( "Clean" )
                .IsDependentOn( "Check-Repository" )
                .Does( () =>
                {
                    Cake.CleanDirectories( "../**/bin/" + configuration, d => !d.Path.Segments.Contains( "build" ) );
                    Cake.CleanDirectories( "../**/obj/" + configuration, d => !d.Path.Segments.Contains( "build" ) );

                    Cake.CleanDirectories( releasesDir );
                    Cake.DeleteFiles( "../Tests/**/TestResult.xml" );
                } );

            Task( "Build" )
                .IsDependentOn( "Clean" )
                .IsDependentOn( "Restore-NuGet-Packages" )
                .IsDependentOn( "Check-Repository" )
                .Does( () =>
                {
                    Cake.DotNetCoreBuild( coreBuildFile,
                      new DotNetCoreBuildSettings().AddVersionArguments( gitInfo, s =>
                       {
                           s.Configuration = configuration;
                       } ) );

                } );

            //Task( "Unit-Testing" )
            //    .IsDependentOn( "Build" )
            //    .Does( () =>
            //    {
            //        Cake.CreateDirectory( releasesDir );
            //        Cake.NUnit( "Tests/*.Tests/bin/" + configuration + "/*.Tests.dll", new NUnitSettings()
            //        {
            //            Framework = "v4.5",
            //            OutputFile = releasesDir.Path + "/TestResult.txt",
            //            StopOnError = true
            //        } );
            //    } );

            Task( "Create-NuGet-Packages" )
                .IsDependentOn( "Build" )
                .WithCriteria( () => gitInfo.IsValid )
                .Does( () =>
                {
                    Cake.CreateDirectory( releasesDir );
                    foreach( SolutionProject p in projectsToPublish )
                    {
                        Cake.Warning( p.Path.GetDirectory().FullPath );
                        var s = new DotNetCorePackSettings()
                        {
                            ArgumentCustomization = args => args.Append( "--include-symbols" ),
                            NoBuild = true,
                            Configuration = configuration,
                            OutputDirectory = releasesDir
                        };
                        s.AddVersionArguments( gitInfo );
                        Cake.DotNetCorePack( p.Path.FullPath, s );
                    }
                } );


            Task( "Push-NuGet-Packages" )
                .IsDependentOn( "Create-NuGet-Packages" )
                .WithCriteria( () => gitInfo.IsValid )
                .Does( () =>
                {
                    IEnumerable<FilePath> nugetPackages = Cake.GetFiles( releasesDir.Path + "/*.nupkg" );
                    if( Cake.IsInteractiveMode() )
                    {
                        var localFeed = Cake.FindDirectoryAbove( "Packarium" );
                        if( localFeed != null )
                        {
                            Cake.Information( "LocalFeed directory found: {0}", localFeed );
                            if( Cake.ReadInteractiveOption( "Do you want to publish to LocalFeed?", 'Y', 'N' ) == 'Y' )
                            {
                                Cake.CopyFiles( nugetPackages, localFeed );
                            }
                        }
                    }
                } );

            // The Default task for this script can be set here.
            Task( "Default" )
                .IsDependentOn( "Push-NuGet-Packages" );

        }
    }
}
