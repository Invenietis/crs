
using Cake.Npm.RunScript;
using Cake.Common.IO;
using Cake.Common.Solution;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.NUnit;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using SimpleGitVersion;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using NuGet.Versioning;

namespace CodeCake
{
    /// <summary>
    /// Standard build "script".
    /// </summary>
    
    public partial class Build : CodeCakeHost
    {
        public Build()
        {
            Cake.Log.Verbosity = Verbosity.Diagnostic;

            StandardGlobalInfo globalInfo = CreateStandardGlobalInfo()
                                                .AddDotnet()
                                                .AddNPM()
                                                .SetCIBuildTag();

            Task("Check-Repository")
                .Does(() =>
               {
                   globalInfo.TerminateIfShouldStop();
               });

            Task( "Clean" )
                .IsDependentOn( "Check-Repository" )
                .Does( () =>
                 {
                     globalInfo.GetDotnetSolution().Clean();
                     Cake.CleanDirectories( globalInfo.ReleasesFolder.ToString() );

                     globalInfo.GetNPMSolution().Clean();
                 } );


            Task("Build")
                .IsDependentOn("Check-Repository")
                .IsDependentOn("Clean")
                .Does(() =>
                {
                    globalInfo.GetDotnetSolution().Build();
                    globalInfo.GetNPMSolution().Build();
                });

            Task("Unit-Testing")
                .IsDependentOn("Build")
                .WithCriteria(() => Cake.InteractiveMode() == InteractiveMode.NoInteraction
                                    || Cake.ReadInteractiveOption("RunUnitTests", "Run Unit Tests?", 'Y', 'N') == 'Y')
               .Does(() =>
               {
                   var testProjects = globalInfo.GetDotnetSolution().Projects.Where(p => p.Name.EndsWith(".Tests")
                                                           && !p.Path.Segments.Contains("Integration"));
                   globalInfo.GetDotnetSolution().Test();
                   globalInfo.GetNPMSolution().Test();
               });

            Task("Create-Packages")
                //.WithCriteria( () => globalInfo.IsValid )
                .IsDependentOn("Unit-Testing")
                .Does(() =>
                {
                    globalInfo.GetDotnetSolution().Pack();
                    globalInfo.GetNPMSolution().RunPack();
                });

            Task("Push-Packages")
                .WithCriteria(() => globalInfo.IsValid)
                .IsDependentOn("Create-Packages")
                .Does( async () =>
                {
                    await globalInfo.PushArtifactsAsync();
                });

            // The Default task for this script can be set here.
            Task("Default")
                .IsDependentOn("Push-Packages");

        }
    }
}
