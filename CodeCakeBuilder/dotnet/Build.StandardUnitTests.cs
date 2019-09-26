using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Solution;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Common.Tools.NUnit;
using Cake.Core.IO;
using CK.Text;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CodeCake
{
    public partial class Build
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="globalInfo"></param>
        /// <param name="testProjects"></param>
        /// <param name="useNUnit264ForNet461">Will use nunit 264 for Net461 project if true.</param>
        [Obsolete( "Use DotnetSolution.Test() instead" )]
        void StandardUnitTests( StandardGlobalInfo globalInfo, IEnumerable<SolutionProject> testProjects )
        {
            globalInfo.GetDotnetSolution().Test();
        }
    }
}
