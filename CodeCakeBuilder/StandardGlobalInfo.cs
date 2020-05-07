using Cake.Common;
using Cake.Common.Build;
using Cake.Common.Build.AppVeyor;
using Cake.Common.Build.TFBuild;
using Cake.Common.Diagnostics;
using Cake.Core;
using CK.Text;
using CodeCake.Abstractions;
using CSemVer;
using SimpleGitVersion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CodeCake
{
    /// <summary>
    /// Exposes global state information for the build script.
    /// </summary>
    public class StandardGlobalInfo
    {
        readonly ICakeContext _ctx;
        readonly HashSet<ICIWorkflow> _solutions = new HashSet<ICIWorkflow>();
        List<ArtifactPush> _artifactPushes;
        bool _ignoreNoArtifactsToProduce;

        static StandardGlobalInfo()
        {
            SharedHttpClient = new HttpClient();
        }

        public StandardGlobalInfo( ICakeContext ctx, ICommitBuildInfo finalBuildInfo )
        {
            _ctx = ctx;
            BuildInfo = finalBuildInfo;
            ReleasesFolder = "CodeCakeBuilder/Releases";
            Directory.CreateDirectory( ReleasesFolder );
        }

        public void RegisterSolution( ICIWorkflow solution )
        {
            _solutions.Add( solution );
        }

        /// <summary>
        /// Gets the Cake context.
        /// </summary>
        public ICakeContext Cake => _ctx;

        /// <summary>
        /// Gets the <see cref="ICommitBuildInfo"/> for this commit.
        /// This holds the <see cref="ICommitBuildInfo.CommitSha"/> and <see cref="ICommitBuildInfo.CommitDateUtc"/> of this commit.
        /// If <see cref="ICommitBuildInfo.Version"/> is the <see cref="SVersion.ZeroVersion"/> then this is not really a
        /// valid build.
        /// </summary>
        public ICommitBuildInfo BuildInfo { get; }

        /// <summary>
        /// Gets whether the <see cref="ICommitBuildInfo.Version"/> is not the <see cref="SVersion.ZeroVersion"/>.
        /// </summary>
        public bool IsValid => BuildInfo.IsValid();

        IEnumerable<ICIPublishWorkflow> SolutionProducingArtifacts => Solutions.OfType<ICIPublishWorkflow>();

        /// <summary>
        /// Gets the set of <see cref="ArtifactType"/> of the <see cref="ICIPublishWorkflow"/> that have been registered.
        /// </summary>
        public IEnumerable<ArtifactType> ArtifactTypes => SolutionProducingArtifacts.Select( p => p.ArtifactType );

        /// <summary>
        /// Gets the release folder: "CodeCakeBuilder/Releases".
        /// </summary>
        public NormalizedPath ReleasesFolder { get; }

        /// <summary>
        /// Gets whether this is a purely local build.
        /// </summary>
        public bool IsLocalCIRelease { get; set; }

        /// <summary>
        /// Shared http client.
        /// See: https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
        /// Do not add any default on it.
        /// </summary>
        public static readonly HttpClient SharedHttpClient;

        /// <summary>
        /// Gets whether artifacts should be pushed to remote feeds.
        /// </summary>
        public bool PushToRemote { get; set; }

        /// <summary>
        /// Gets or sets the local feed path.
        /// Can be null if no local feed exists or if local feed should be ignored.
        /// </summary>
        public string LocalFeedPath { get; set; }

        /// <summary>
        /// Gets or sets whether <see cref="NoArtifactsToProduce"/> should be ignored.
        /// Defaults to false: by default if there is no packages to produce <see cref="ShouldStop"/> is true.
        /// </summary>
        public bool IgnoreNoArtifactsToProduce
        {
            get
            {
                return Cake.Argument( "IgnoreNoArtifactsToProduce", 'N' ) == 'Y' || _ignoreNoArtifactsToProduce;
            }
            set
            {
                _ignoreNoArtifactsToProduce = value;
            }
        }

        /// <summary>
        /// Gets whether <see cref="GetArtifactPushList"/> is empty: all artifacts are already available in all
        /// artifact repositories: unless <see cref="IgnoreNoArtifactsToProduce"/> is set to true, there is nothing to
        /// do and <see cref="ShouldStop"/> is true.
        /// </summary>
        public bool NoArtifactsToProduce => !GetArtifactPushList().Any();

        /// <summary>
        /// Gets a read only list of all the pushes of artifacts for all <see cref="ArtifactType"/>.
        /// </summary>
        /// <param name="reset">
        /// True to recompute the list from all <see cref="ArtifactType.GetPushListAsync(bool)"/>
        /// (without reseting them).
        /// </param>
        /// <returns>The set of pushes.</returns>
        public IReadOnlyList<ArtifactPush> GetArtifactPushList( bool reset = false )
        {
            if( _artifactPushes == null || reset )
            {
                _artifactPushes = new List<ArtifactPush>();
                var tasks = ArtifactTypes.Select( f => f.GetPushListAsync() ).ToArray();
                Task.WaitAll( tasks );
                foreach( var p in tasks.Select( t => t.Result ) )
                {
                    _artifactPushes.AddRange( p );
                }
            }
            return _artifactPushes;
        }

        /// <summary>
        /// Gets whether there is no artifact to produce and <see cref="IgnoreNoArtifactsToProduce"/> is false.
        /// </summary>
        public bool ShouldStop => NoArtifactsToProduce && !IgnoreNoArtifactsToProduce;

        public IReadOnlyCollection<ICIWorkflow> Solutions => _solutions;

        #region Memory key support.

        string MemoryFilePath => $"CodeCakeBuilder/MemoryKey.{BuildInfo.CommitSha}.txt";

        public void WriteCommitMemoryKey( NormalizedPath key )
        {
            if( BuildInfo.IsValid() ) File.AppendAllLines( MemoryFilePath, new[] { key.ToString() } );
        }

        public bool CheckCommitMemoryKey( NormalizedPath key )
        {
            bool done = File.Exists( MemoryFilePath )
                        ? Array.IndexOf( File.ReadAllLines( MemoryFilePath ), key.Path ) >= 0
                        : false;
            if( done )
            {
                if( !BuildInfo.IsValid() )
                {
                    Cake.Information( $"Zero commit. Key exists but is ignored: {key}" );
                    done = false;
                }
                else
                {
                    Cake.Information( $"Key exists on this commit: {key}" );
                }
            }
            return done;
        }

        #endregion

        /// <summary>
        /// Simply calls <see cref="ArtifactType.PushAsync(IEnumerable{ArtifactPush})"/> on each <see cref="ArtifactTypes"/>
        /// with their correct typed artifacts.
        /// </summary>
        public void PushArtifacts( IEnumerable<ArtifactPush> pushes = null )
        {
            if( pushes == null ) pushes = GetArtifactPushList();
            var tasks = ArtifactTypes.Select( t => t.PushAsync( pushes.Where( a => a.Feed.ArtifactType == t ) ) ).ToArray();
            Task.WaitAll( tasks );
        }

        /// <summary>
        /// Tags the build when running on Appveyor or AzureDevOps (GitLab does not support this).
        /// Note that if <see cref="ShouldStop"/> is true, " (Skipped)" is appended.
        /// </summary>
        /// <returns>This info object (allowing fluent syntax).</returns>
        public StandardGlobalInfo SetCIBuildTag()
        {
            string AddSkipped( string s ) => ShouldStop ? s + " (Skipped)" : s;

            string ComputeAzurePipelineUpdateBuildVersion( ICommitBuildInfo buildInfo )
            {
                // Azure (formerly VSTS, formerly VSO) analyzes the stdout to set its build number.
                // On clash, the default Azure/VSTS/VSO build number is used: to ensure that the actual
                // version will be always be available we need to inject a uniquifier.
                string buildVersion = AddSkipped( $"{buildInfo.Version}_{DateTime.UtcNow:yyyyMMdd-HHmmss}" );
                Cake.Information( $"Using VSTS build number: {buildVersion}" );
                return $"##vso[build.updatebuildnumber]{buildVersion}";
            }

            void AzurePipelineUpdateBuildVersion( string buildInstruction )
            {
                Console.WriteLine();
                Console.WriteLine( buildInstruction );
                Console.WriteLine();
            }

            IAppVeyorProvider appVeyor = Cake.AppVeyor();
            ITFBuildProvider vsts = Cake.TFBuild();
            try
            {
                if( appVeyor.IsRunningOnAppVeyor )  
                {
                    appVeyor.UpdateBuildVersion( AddSkipped( BuildInfo.Version.ToString() ) );
                }

                if( vsts.IsRunningOnAzurePipelinesHosted || vsts.IsRunningOnAzurePipelines )
                {
                    string azureVersion = ComputeAzurePipelineUpdateBuildVersion( BuildInfo );
                    AzurePipelineUpdateBuildVersion( azureVersion );
                }
            }
            catch( Exception e )
            {
                Cake.Warning( "Could not set the Build Version !!!" );
                Cake.Warning( e );
            }

            return this;
        }

        /// <summary>
        /// Terminates the script with success if <see cref="ShouldStop"/> is true.
        /// (Calls <see cref="TerminateAliases.TerminateWithSuccess(ICakeContext, string)">Cake.TerminateWithSuccess</see>.)
        /// </summary>
        /// <returns>This info object (allowing fluent syntax).</returns>
        public StandardGlobalInfo TerminateIfShouldStop()
        {
            if( ShouldStop )
            {
                Cake.TerminateWithSuccess( "All packages from this commit are already available. Build skipped." );
            }
            return this;
        }

    }

}
