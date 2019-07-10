using CodeCake.Abstractions;
using System.Collections.Generic;
using System.Linq;
using Cake.Npm;
using Cake.Common.Diagnostics;
using CSemVer;

namespace CodeCake
{
    public static class StandardGlobalInfoNPMExtension
    {
        /// <summary>
        /// Adds the <see cref="Build.NPMArtifactType"/> for NPM artifacts.
        /// </summary>
        /// <param name="this">This global info.</param>
        /// <param name="solution">The NPM solution.</param>
        /// <returns>This info.</returns>
        public static StandardGlobalInfo AddNPM( this StandardGlobalInfo globalInfo, NPMSolution solution )
        {
            SVersion minmimalNpmVersionRequired = SVersion.Create( 6, 7, 0 );
            string npmVersion = globalInfo.Cake.NpmGetNpmVersion();
            if( SVersion.Parse( npmVersion ) < minmimalNpmVersionRequired )
            {
                globalInfo.Cake.TerminateWithError( "Outdated npm. Version older than this are known to fail on publish." );
            }
            new Build.NPMArtifactType( globalInfo, solution );
            return globalInfo;
        }

        /// <summary>
        /// Adds the <see cref="Build.NPMArtifactType"/> for NPM based on <see cref="NPMSolution.ReadFromNPMSolutionFile"/>
        /// (projects are defined by "CodeCakeBuilder/NPMSolution.xml" file).
        /// </summary>
        /// <param name="this">This global info.</param>
        /// <returns>This info.</returns>
        public static StandardGlobalInfo AddNPM( this StandardGlobalInfo @this )
        {
            return AddNPM( @this, NPMSolution.ReadFromNPMSolutionFile( @this ) );
        }

        /// <summary>
        /// Gets the NPM solution handled by the single <see cref="Build.NPMArtifactType"/>.
        /// </summary>
        /// <param name="this">This global info.</param>
        /// <returns>The NPM solution.</returns>
        public static NPMSolution GetNPMSolution( this StandardGlobalInfo @this )
        {
            return @this.ArtifactTypes.OfType<Build.NPMArtifactType>().Single().Solution;
        }
    }

    public partial class Build
    {
        /// <summary>
        /// Supports NPM packages.
        /// </summary>
        public class NPMArtifactType : ArtifactType
        {
            public NPMArtifactType( StandardGlobalInfo globalInfo, NPMSolution solution )
                : base( globalInfo, "NPM" )
            {
                Solution = solution;
            }

            public NPMSolution Solution { get; }

            protected override IEnumerable<ILocalArtifact> GetLocalArtifacts() => Solution.PublishedProjects;


            protected override IEnumerable<ArtifactFeed> GetRemoteFeeds()
            {
                yield return new AzureNPMFeed( this, "Signature-OpenSource", "Default" );
if( GlobalInfo.Version.PackageQuality >= CSemVer.PackageQuality.ReleaseCandidate ) yield return new NPMRemoteFeed( this, "NPMJS_ORG_PUSH_PAT", "https://registry.npmjs.org/", false );

            }

            protected override IEnumerable<ArtifactFeed> GetLocalFeeds()
            {
                return new ArtifactFeed[] {
                    new NPMLocalFeed( this, GlobalInfo.LocalFeedPath )
                };
            }
        }
    }
}
