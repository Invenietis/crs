using Cake.Common.Build;
using Cake.Common.Diagnostics;
using SimpleGitVersion;

namespace CodeCake
{
    public partial class Build
    {
        /// <summary>
        /// Creates a new <see cref="StandardGlobalInfo"/> initialized by the
        /// current environment.
        /// </summary>
        /// <returns>A new info object.</returns>
        StandardGlobalInfo CreateStandardGlobalInfo()
        {
            var result = new StandardGlobalInfo( Cake, Cake.GetRepositoryInfo().FinalBuildInfo );
            // By default:
            if( result.IsValid )
            {
                // gitInfo is valid: it is either ci or a release build. 
                var v = result.BuildInfo.Version;
                // If a /LocalFeed/ directory exists above, we publish the packages in it.
                var localFeedRoot = Cake.FindSiblingDirectoryAbove( Cake.Environment.WorkingDirectory.FullPath, "LocalFeed" );
                if( localFeedRoot != null )
                {
                    if( v.AsCSVersion == null )
                    {
                        if( v.Prerelease.EndsWith( ".local" ) )
                        {
                            // Local releases must not be pushed on any remote and are copied to LocalFeed/Local
                            // feed (if LocalFeed/ directory above exists).
                            result.IsLocalCIRelease = true;
                            result.LocalFeedPath = System.IO.Path.Combine( localFeedRoot, "Local" );
                        }
                        else
                        {
                            // CI build versions are routed to LocalFeed/CI
                            result.LocalFeedPath = System.IO.Path.Combine( localFeedRoot, "CI" );
                        }
                    }
                    else
                    {
                        // Release or prerelease go to LocalFeed/Release
                        result.LocalFeedPath = System.IO.Path.Combine( localFeedRoot, "Release" );
                    }
                    System.IO.Directory.CreateDirectory( result.LocalFeedPath );
                }
                else result.IsLocalCIRelease = v.Prerelease.EndsWith( ".local" );

                // Creating the right remote feed.
                if( !result.IsLocalCIRelease
                    && (Cake.InteractiveMode() == InteractiveMode.NoInteraction
                        || Cake.ReadInteractiveOption( "PushToRemote", "Push to Remote feeds?", 'Y', 'N' ) == 'Y') )
                {
                    result.PushToRemote = true;
                }
            }
            else
            {
                if( Cake.InteractiveMode() != InteractiveMode.NoInteraction
                    && Cake.ReadInteractiveOption( "PublishDirtyRepo", "Repository is not ready to be published. Proceed anyway?", 'Y', 'N' ) == 'Y' )
                {
                    Cake.Warning( "Unable to compute a valid version, but you choose to continue..." );
                    result.IgnoreNoArtifactsToProduce = true;
                }
                else
                {
                    // On Appveyor, we let the build run: this gracefully handles Pull Requests.
                    if( Cake.AppVeyor().IsRunningOnAppVeyor )
                    {
                        result.IgnoreNoArtifactsToProduce = true;
                    }
                    else
                    {
                        Cake.TerminateWithError( "Repository is not ready to be published." );
                    }
                }
                // When the gitInfo is not valid, we do not try to push any packages, even if the build continues
                // (either because the user choose to continue or if we are on the CI server).
                // We don't need to worry about feeds here.
            }
            return result;
        }

    }
}
