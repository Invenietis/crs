using Cake.Common.Solution;
using Cake.Core;
using CodeCake.Abstractions;
using CSemVer;
using System.Collections.Generic;
using System.Linq;

namespace CodeCake
{
    public static class StandardGlobalInfoNuGetExtension
    {
        public static StandardGlobalInfo AddNuGet( this StandardGlobalInfo globalInfo, IEnumerable<SolutionProject> projectsToPublish )
        {
            new Build.NuGetArtifactType( globalInfo, projectsToPublish );
            return globalInfo;
        }
    }

    public partial class Build
    {
        /// <summary>
        /// Implements NuGet package handling.
        /// </summary>
        public class NuGetArtifactType : ArtifactType
        {
            readonly IList<SolutionProject> _projectsToPublish;

            public class NuGetArtifact : ILocalArtifact
            {
                public NuGetArtifact( SolutionProject p, SVersion v )
                {
                    Project = p;
                    ArtifactInstance = new ArtifactInstance( "NuGet", p.Name, v );
                }

                public ArtifactInstance ArtifactInstance { get; }

                public SolutionProject Project { get; }
            }

            public NuGetArtifactType( StandardGlobalInfo globalInfo, IEnumerable<SolutionProject> projectsToPublish )
                : base( globalInfo, "NuGet" )
            {
                _projectsToPublish = projectsToPublish.ToList();
            }

            /// <summary>
            /// Downcasts the mutable list <see cref="ILocalArtifact"/> as a set of <see cref="NuGetArtifact"/>.
            /// </summary>
            /// <returns>The set of NuGet artifacts.</returns>
            public IEnumerable<NuGetArtifact> GetNuGetArtifacts() => GetArtifacts().Cast<NuGetArtifact>();

            /// <summary>
            /// Gets the remote target feeds.
            /// </summary>
            /// <returns>The set of remote NuGet feeds (in practice at most one).</returns>
            protected override IEnumerable<ArtifactFeed> GetRemoteFeeds()
            {
                if( GlobalInfo.Version.PackageQuality >= PackageQuality.ReleaseCandidate ) yield return new RemoteFeed( this, "nuget.org", "https://api.nuget.org/v3/index.json", "NUGET_ORG_PUSH_API_KEY" );
yield return new SignatureVSTSFeed( this, "Signature-OpenSource", "Default" );

            }

            /// <summary>
            /// Gets the local target feeds.
            /// </summary>
            /// <returns>The set of remote NuGet feeds (in practice at moste one).</returns>
            protected override IEnumerable<ArtifactFeed> GetLocalFeeds()
            {
                return new NuGetHelper.NuGetFeed[] {
                    new NugetLocalFeed( this, GlobalInfo.LocalFeedPath )
                };
            }

            protected override IEnumerable<ILocalArtifact> GetLocalArtifacts()
            {
                return _projectsToPublish.Select( p => new NuGetArtifact( p, GlobalInfo.Version ) );
            }

        }
    }
}
