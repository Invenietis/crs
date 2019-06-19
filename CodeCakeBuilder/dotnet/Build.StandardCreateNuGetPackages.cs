using Cake.Common.Diagnostics;
using Cake.Common.Solution;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Pack;
using Cake.Core;
using Cake.Core.IO;
using SimpleGitVersion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeCake
{
    public partial class Build
    {
        void StandardCreateNuGetPackages( StandardGlobalInfo globalInfo )
        {
            StandardCreateNuGetPackages( globalInfo.ArtifactTypes.OfType<NuGetArtifactType>().Single() );
        }

        void StandardCreateNuGetPackages( NuGetArtifactType nugetInfo )
        {
            var settings = new DotNetCorePackSettings().AddVersionArguments( nugetInfo.GlobalInfo.GitInfo, c =>
            {
                // IsPackable=true is required for Tests package. Without this Pack on Tests projects
                // does not generate nupkg.
                c.ArgumentCustomization += args => args.Append( "/p:IsPackable=true" );
                c.NoBuild = true;
                c.IncludeSymbols = true;
                c.Configuration = nugetInfo.GlobalInfo.BuildConfiguration;
                c.OutputDirectory = nugetInfo.GlobalInfo.ReleasesFolder.Path;
            } );
            foreach( var p in nugetInfo.GetNuGetArtifacts() )
            {
                Cake.Information( p.ArtifactInstance );
                Cake.DotNetCorePack( p.Project.Path.FullPath, settings );
            }
        }
    }
}
