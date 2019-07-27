using CodeCake.Abstractions;
using System.Collections.Generic;
using System.Linq;
using Cake.Npm;
using Cake.Common.Diagnostics;
using CSemVer;
using static CodeCake.Build;
using CodeCake;

namespace CodeCake
{

    public partial class NPMSolution : ISolutionProducingArtifact
    {
        private ArtifactType _artifactType;

        public ArtifactType ArtifactType
        {
            get
            {
                if( _artifactType == null ) _artifactType = new NPMArtifactType(_globalInfo, this);
                return _artifactType;
            }
        }

        public void Pack() => RunPack();
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
