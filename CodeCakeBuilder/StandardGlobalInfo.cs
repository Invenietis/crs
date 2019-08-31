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
        readonly SimpleRepositoryInfo _gitInfo;
        readonly HashSet<ISolution> _solutions = new HashSet<ISolution>();
        List<ArtifactPush> _artifactPushes;
        bool _ignoreNoArtifactsToProduce;

        static StandardGlobalInfo()
        {
            SharedHttpClient = new HttpClient();
        }
        public StandardGlobalInfo( ICakeContext ctx, SimpleRepositoryInfo gitInfo )
        {
            _ctx = ctx;
            _gitInfo = gitInfo;
            // We build in Debug for any prerelease except "rc": the last prerelease step is in Release.
            IsRelease = gitInfo.IsValidRelease
                           && (gitInfo.PreReleaseName.Length == 0 || gitInfo.PreReleaseName == "rc");
            ReleasesFolder = "CodeCakeBuilder/Releases";
            Directory.CreateDirectory( ReleasesFolder );
        }

        public void RegisterSolution( ISolution solution )
        {
            _solutions.Add( solution );
        }

        /// <summary>
        /// Gets the Cake context.
        /// </summary>
        public ICakeContext Cake => _ctx;

        /// <summary>
        /// Gets the SimpleRepositoryInfo.
        /// </summary>
        public SimpleRepositoryInfo GitInfo => _gitInfo;

        IEnumerable<ISolutionProducingArtifact> SolutionProducingArtifacts => Solutions.OfType<ISolutionProducingArtifact>();

        /// <summary>
        /// Gets the set of <see cref="ArtifactType"/> of the <see cref="ISolutionProducingArtifact"/> that have been registered.
        /// </summary>
        public IEnumerable<ArtifactType> ArtifactTypes => SolutionProducingArtifacts.Select( p => p.ArtifactType );

        /// <summary>
        /// Gets the release folder: "CodeCakeBuilder/Releases".
        /// </summary>
        public NormalizedPath ReleasesFolder { get; }

        /// <summary>
        /// Gets or sets if the build is a release
        /// By defaults, a <see cref="PackageQuality.Release"/> and "rc" prerealease are in "Release" and
        /// all other versions are in "Debug".
        /// </summary>
        public bool IsRelease { get; set; }

        /// <summary>
        /// Gets the "Release" or "Debug" based on <see cref="IsRelease"/>.
        /// </summary>
        public string BuildConfiguration => IsRelease ? "Release" : "Debug";

        /// <summary>
        /// Gets the version of the packages: this is the <see cref="RepositoryInfo.FinalVersion"/>.
        /// </summary>
        public SVersion Version => _gitInfo.Info.FinalVersion;

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

        public IReadOnlyCollection<ISolution> Solutions => _solutions;

        #region Memory key support.

        string MemoryFilePath => $"CodeCakeBuilder/MemoryKey.{GitInfo.CommitSha}.txt";

        public void WriteCommitMemoryKey( NormalizedPath key )
        {
            if( GitInfo.IsValid ) File.AppendAllLines( MemoryFilePath, new[] { key.ToString() } );
        }

        public bool CheckCommitMemoryKey( NormalizedPath key )
        {
            bool done = File.Exists( MemoryFilePath )
                        ? Array.IndexOf( File.ReadAllLines( MemoryFilePath ), key.Path ) >= 0
                        : false;
            if( done )
            {
                if( !GitInfo.IsValid )
                {
                    Cake.Information( $"Dirty commit. Key exists but is ignored: {key}" );
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

            void AzurePipelineUpdateBuildVersion( SimpleRepositoryInfo gitInfo )
            {
                // Azure (formerly VSTS, formerly VSO) analyzes the stdout to set its build number.
                // On clash, the default Azure/VSTS/VSO build number is used: to ensure that the actual
                // version will be always be available we need to inject a uniquifier.
                string buildVersion = AddSkipped( $"{gitInfo.SafeVersion}_{DateTime.UtcNow:yyyyMMdd-HHmmss}" );
                Cake.Information( $"Using VSTS build number: {buildVersion}" );
                string buildInstruction = $"##vso[build.updatebuildnumber]{buildVersion}";
                Console.WriteLine();
                Console.WriteLine( buildInstruction );
                Console.WriteLine();
            }

            void AppVeyorUpdateBuildVersion( IAppVeyorProvider appVeyor, SimpleRepositoryInfo gitInfo )
            {
                try
                {
                    appVeyor.UpdateBuildVersion( AddSkipped( gitInfo.SafeVersion ) );
                }
                catch
                {
                    appVeyor.UpdateBuildVersion( AddSkipped( $"{gitInfo.SafeVersion} ({appVeyor.Environment.Build.Number})" ) );
                }
            }

            var gitlab = Cake.GitLabCI();
            if( gitlab.IsRunningOnGitLabCI )
            {
                // damned, we can't tag the pipeline/job.
            }
            else
            {
                IAppVeyorProvider appVeyor = Cake.AppVeyor();
                if( appVeyor.IsRunningOnAppVeyor )
                {
                    AppVeyorUpdateBuildVersion( appVeyor, _gitInfo );
                }
                else
                {
                    ITFBuildProvider vsts = Cake.TFBuild();
                    if( vsts.IsRunningOnAzurePipelinesHosted || vsts.IsRunningOnAzurePipelines )
                    {
                        AzurePipelineUpdateBuildVersion( _gitInfo );
                    }
                }
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
