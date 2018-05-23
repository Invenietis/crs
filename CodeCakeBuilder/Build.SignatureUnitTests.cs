using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Solution;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Common.Tools.NUnit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CodeCake
{
    public partial class Build
    {
        void SignatureUnitTests( string configuration, IEnumerable<SolutionProject> testProjects )
        {
            var resultsDirectory = Cake.Directory( "CodeCakeBuilder/TestResult" );
            if(Cake.DirectoryExists( resultsDirectory ))
            {
                Cake.DeleteDirectory( resultsDirectory, new DeleteDirectorySettings() { Force = true, Recursive = true } );
            }
            Cake.CreateDirectory( resultsDirectory );

            foreach( var p in testProjects.Where( p => p.Name.EndsWith( ".Tests" ) ) )
            {
                Cake.Information( "Testing: {0}", p.Name );
                // Tests all target frameworks
                Cake.DotNetCoreTest( p.Path.FullPath, new DotNetCoreTestSettings
                {
                    Configuration = configuration,
                    NoBuild = true,
                    ResultsDirectory = resultsDirectory,
                    Logger = "trx",
                } );
            }
        }
    }
}
