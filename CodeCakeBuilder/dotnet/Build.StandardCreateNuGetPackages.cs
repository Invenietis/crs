using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Pack;
using Cake.Core;
using SimpleGitVersion;
using System;
using System.Linq;

namespace CodeCake
{
    public partial class Build
    {
        [Obsolete( "Use DotnetSolution.Pack() instead")]
        void StandardCreateNuGetPackages( StandardGlobalInfo globalInfo )
        {
            globalInfo.GetDotnetSolution().Pack();
        }
        [Obsolete( "Use DotnetSolution.Pack() instead") ]
        void StandardCreateNuGetPackages( NuGetArtifactType nugetInfo )
        {
            nugetInfo.GlobalInfo.GetDotnetSolution().Pack();
        }
    }
}
