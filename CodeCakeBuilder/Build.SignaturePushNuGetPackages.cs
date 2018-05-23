using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.NuGet;
using Cake.Common.Tools.NuGet.Push;
using Cake.Core.IO;
using SimpleGitVersion;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CodeCake
{
    public partial class Build
    {
        const string NUGET_OUTPUT_DIR = "NuGetPackages";
        const string RELEASE_FEED_NAME = "Signature";
        const string CI_FEED_NAME = "SignatureCI";

        void SignaturePushNuGetPackages( IEnumerable<FilePath> nugetPackages, SimpleRepositoryInfo gitInfo )
        {
            Cake.Warning( "Packages must be transferred to VSTS feeds with a build step on VSTS itself." );
            Cake.Warning( $"Please ensure you have a VSTS build step that pushes directory {NUGET_OUTPUT_DIR}/{RELEASE_FEED_NAME} to feed {RELEASE_FEED_NAME}, and {NUGET_OUTPUT_DIR}/{CI_FEED_NAME} to {CI_FEED_NAME}." );

            // Clean-up, to avoid pushing old packages
            var outputDir = Cake.Directory( NUGET_OUTPUT_DIR );
            if( Cake.DirectoryExists( outputDir ) )
            {
                Cake.DeleteDirectory( outputDir, new DeleteDirectorySettings() { Recursive = true, Force = true } );
            }

            if( Cake.IsInteractiveMode() )
            {
                var localFeed = Cake.FindDirectoryAbove( "LocalFeed" );
                if( localFeed != null )
                {
                    Cake.Information( $"LocalFeed directory found: {localFeed}" );
                    if( Cake.ReadInteractiveOption( "LocalFeed", "Do you want to publish to LocalFeed?", 'Y', 'N' ) == 'Y' )
                    {
                        Cake.CopyFiles( nugetPackages, localFeed );
                    }
                }
            }

            DirectoryPath targetDir;

            if( gitInfo.IsValidRelease )
            {
                targetDir = Cake.Directory( $"{NUGET_OUTPUT_DIR}/{RELEASE_FEED_NAME}" );
            }
            else
            {
                Debug.Assert( gitInfo.IsValidCIBuild );
                targetDir = Cake.Directory( $"{NUGET_OUTPUT_DIR}/{CI_FEED_NAME}" );
            }

            Cake.CreateDirectory( targetDir );
            Cake.CopyFiles( nugetPackages, targetDir );

            SetVstsBuildNumber( gitInfo );
        }

        void SetVstsBuildNumber( SimpleRepositoryInfo gitInfo )
        {
            // Build numbers are updated through a an line in stdout (##vso[...]), which is then parsed by VSTS.
            string vstsBuildNumber = $"{gitInfo.SafeNuGetVersion}_{DateTime.UtcNow.ToString( "yyyyMMdd-HHmmss" )}";

            Cake.Information( $"Using VSTS build number: {vstsBuildNumber}" );
            string buildNumberInstruction = $"##vso[build.updatebuildnumber]{vstsBuildNumber}";

            Console.WriteLine();
            Console.WriteLine( buildNumberInstruction );
            Console.WriteLine();
        }

    }
}
