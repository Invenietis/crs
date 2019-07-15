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
    [AddPath("%UserProfile%/.nuget/packages/**/tools*")]
    public partial class Build : CodeCakeHost
    {
        public Build()
        {
            Cake.Log.Verbosity = Verbosity.Diagnostic;

            SimpleRepositoryInfo gitInfo = Cake.GetSimpleRepositoryInfo();
            StandardGlobalInfo globalInfo = CreateStandardGlobalInfo(gitInfo)
                                                .AddDotnet()
                                                .AddNPM()
                                                .SetCIBuildTag();

            Task("Check-Repository")
                .Does(() =>
               {
                   globalInfo.TerminateIfShouldStop();
               });

            Task("Clean")
                .IsDependentOn("Check-Repository")
                .Does(() =>
                {
                    globalInfo.GetDotnetSolution().Clean();
                    Cake.CleanDirectories(globalInfo.ReleasesFolder);

                    globalInfo.GetNPMSolution().RunInstallAndClean(scriptMustExist: false);
                });


            Task("Build")
                .IsDependentOn("Check-Repository")
                .IsDependentOn("Clean")
                .Does(() =>
                {
                    globalInfo.GetDotnetSolution().Build();
                    globalInfo.GetNPMSolution().RunBuild();
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
                   globalInfo.GetNPMSolution().RunTest();
               });

            Task("Create-Packages")
                //.WithCriteria( () => gitInfo.IsValid )
                .IsDependentOn("Unit-Testing")
                .Does(() =>
                {
                    globalInfo.GetDotnetSolution().Pack();
                    globalInfo.GetNPMSolution().RunPack(false);
                });

            Task("Push-Packages")
                .WithCriteria(() => gitInfo.IsValid)
                .IsDependentOn("Create-Packages")
                .Does(() =>
                {
                    globalInfo.PushArtifacts();
                });

            // The Default task for this script can be set here.
            Task("Default")
                .IsDependentOn("Push-Packages");

        }

        private void UpdateLocalNpmVersions(StandardGlobalInfo globalInfo, JObject obj)
        {
            var npmArtifact = globalInfo.ArtifactTypes.FirstOrDefault(x => x.TypeName == "NPM") as NPMArtifactType;
            var dependencyPropNames = new string[]
            {
                "dependencies",
                "peerDependencies",
                "devDependencies",
                "bundledDependencies",
                "optionalDependencies",
            };

            foreach (var dependencyPropName in dependencyPropNames)
            {
                if (obj.ContainsKey(dependencyPropName))
                {
                    FixupLocalNpmVersion((JObject)obj[dependencyPropName], npmArtifact);
                }
            }
        }


        private void FixupLocalNpmVersion(JObject dependencies, NPMArtifactType npmArtifactType)
        {
            foreach (KeyValuePair<string, JToken> keyValuePair in dependencies)
            {
                var localProject = npmArtifactType?.Solution?.Projects.FirstOrDefault(x => x.PackageJson.Name == keyValuePair.Key)
                    as NPMPublishedProject;

                if (localProject != null)
                {
                    dependencies[keyValuePair.Key] = new JValue("^" + localProject.ArtifactInstance.Version.ToNuGetPackageString());
                }
            }
        }
    }
}
