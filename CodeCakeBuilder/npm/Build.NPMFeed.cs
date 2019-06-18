using System;
using System.Collections.Generic;
using Cake.Common.IO;
using Cake.Npm;
using Cake.Npm.Publish;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;
using CSemVer;
using Cake.Core;
using CodeCake.Abstractions;
using SimpleGitVersion;
using Cake.Common.Diagnostics;
using CK.Text;

namespace CodeCake
{
    public partial class Build
    {
        abstract class NPMFeed : ArtifactFeed
        {
            public NPMFeed( NPMArtifactType t )
                : base( t )
            {
            }

            /// <summary>
            /// Pushes a set of artifacts.
            /// </summary>
            /// <param name="pushes">The instances to push (that necessary target this feed).</param>
            /// <returns>The awaitable.</returns>
            public override async Task PushAsync( IEnumerable<ArtifactPush> pushes )
            {
                var tasks = pushes.Select( p => PublishOnePackageAsync( p ) ).ToArray();
                await System.Threading.Tasks.Task.WhenAll( tasks );
            }

            protected abstract Task PublishOnePackageAsync( ArtifactPush push );

        }

        class NPMLocalFeed : NPMFeed
        {
            readonly NormalizedPath _pathToLocalFeed;

            public override string Name => _pathToLocalFeed;

            public NPMLocalFeed( NPMArtifactType t, NormalizedPath pathToLocalFeed )
                : base( t )
            {
                _pathToLocalFeed = pathToLocalFeed;
            }

            public override Task<IEnumerable<ArtifactPush>> CreatePushListAsync( IEnumerable<ILocalArtifact> artifacts )
            {
                var result = artifacts.Cast<NPMPublishedProject>()
                                .Select( a => (A: a, P: _pathToLocalFeed.AppendPart( a.TGZName ) ) )
                                .Where( aP => !File.Exists( aP.P ) )
                                .Select( aP => new ArtifactPush( aP.A, this ) )
                                .ToList();
                return System.Threading.Tasks.Task.FromResult<IEnumerable<ArtifactPush>>( result );
            }

            protected override Task PublishOnePackageAsync( ArtifactPush p )
            {
                if( p.Feed != this ) throw new ArgumentException( "ArtifactPush: feed mismatch." );
                var project = (NPMPublishedProject)p.LocalArtifact;
                var target = _pathToLocalFeed.AppendPart( project.TGZName );
                var source = ArtifactType.GlobalInfo.ReleasesFolder.AppendPart( project.TGZName );
                Cake.CopyFile( source.Path, target.Path );
                return System.Threading.Tasks.Task.CompletedTask;
            }
        }

        abstract class NPMRemoteFeedBase : NPMFeed
        {
            protected readonly string FeedUri;

            public NPMRemoteFeedBase( NPMArtifactType t, string secretKeyName, string feedUri )
                : base( t )
            {
                SecretKeyName = secretKeyName;
                FeedUri = feedUri;
            }

            public override string Name => FeedUri;

            public string SecretKeyName { get; }

            public string ResolveAPIKey()
            {
                string output = Cake.InteractiveEnvironmentVariable( SecretKeyName );
                if( string.IsNullOrWhiteSpace( output ) )
                {
                    Cake.Warning( "Missing environement variable " + SecretKeyName );
                }
                return output;
            }

            protected abstract IDisposable TokenInjector( NPMPublishedProject project );

            public override Task<IEnumerable<ArtifactPush>> CreatePushListAsync( IEnumerable<ILocalArtifact> artifacts )
            {
                var result = artifacts.Cast<NPMPublishedProject>()
                         .Where( a =>
                            {
                                using( TokenInjector( a ) )
                                {
                                    string viewString = Cake.NpmView( a.Name, a.DirectoryPath );
                                    if( string.IsNullOrEmpty( viewString ) ) return true;
                                    JObject json = JObject.Parse( viewString );
                                    if( json.TryGetValue( "versions", out JToken versions ) )
                                    {
                                        return !((JArray)versions).ToObject<string[]>().Contains( a.ArtifactInstance.Version.ToString() );
                                    }
                                    return true;
                                }
                            } )
                         .Select( a => new ArtifactPush( a, this ) )
                         .ToList();
                return System.Threading.Tasks.Task.FromResult<IEnumerable<ArtifactPush>>( result );
            }

            protected override Task PublishOnePackageAsync( ArtifactPush p )
            {
                var tags = p.Version.PackageQuality.GetLabels().Select( l => l.ToString() ).ToList();
                var project = (NPMPublishedProject)p.LocalArtifact;
                using( TokenInjector( project ) )
                {
                    var absTgzPath = Path.GetFullPath( ArtifactType.GlobalInfo.ReleasesFolder.AppendPart( project.TGZName ) );
                    Cake.NpmPublish(
                        new NpmPublishSettings()
                        {
                            Source = absTgzPath,
                            WorkingDirectory = project.DirectoryPath.Path,
                            Tag = tags.First()
                        } );
                    foreach( string tag in tags.Skip( 1 ) )
                    {
                        Cake.Information( $"Adding tag \"{tag}\" to \"{project.Name}@{project.ArtifactInstance.Version}\"..." );
                        // The FromPath is actually required - if executed outside the relevant directory,
                        // it will miss the .npmrc with registry configs.
                        Cake.NpmDistTagAdd( project.Name, project.ArtifactInstance.Version.ToString(), tag, s => s.FromPath( project.DirectoryPath.Path ) );
                    }
                }
                return System.Threading.Tasks.Task.CompletedTask;
            }
        }

        /// <summary>
        /// Feed for a standard NPM server.
        /// </summary>
        class NPMRemoteFeed : NPMRemoteFeedBase
        {
            readonly bool _usePassword;

            public NPMRemoteFeed( NPMArtifactType t, string secretKeyName, string feedUri, bool usePassword )
                : base( t, secretKeyName, feedUri )
            {
                _usePassword = usePassword;
            }

            protected override IDisposable TokenInjector( NPMPublishedProject project )
            {
                return _usePassword
                        ? project.TemporarySetPushTargetAndPasswordLogin( FeedUri, ResolveAPIKey() )
                        : project.TemporarySetPushTargetAndTokenLogin( FeedUri, ResolveAPIKey() );
            }
        }

        /// <summary>
        /// The secret key name is built by <see cref="SignatureVSTSFeed.GetSecretKeyName"/>:
        /// "AZURE_FEED_" + Organization.ToUpperInvariant().Replace( '-', '_' ).Replace( ' ', '_' ) + "_PAT".
        /// </summary>
        /// <param name="organization">Name of the organization.</param>
        /// <param name="feedName">Identifier of the feed in Azure, inside the organization.</param>
        class AzureNPMFeed : NPMRemoteFeedBase
        {
            /// <summary>
            /// Builds the standardized secret key name from the organization name: this is
            /// the Personal Access Token that allows push packages.
            /// </summary>
            /// <param name="organization">Organization name.</param>
            /// <returns>The secret key name to use.</returns>
            public static string GetSecretKeyName( string organization )
                                    => "AZURE_FEED_" + organization.ToUpperInvariant()
                                                                   .Replace( '-', '_' )
                                                                   .Replace( ' ', '_' )
                                                     + "_PAT";

            public AzureNPMFeed( NPMArtifactType t, string organization, string feedName )
                : base( t,
                        GetSecretKeyName( organization ),
                        $"https://pkgs.dev.azure.com/{organization}/_packaging/{feedName}/npm/registry/" )
            {
                Organization = organization;
                FeedName = feedName;
            }

            public string Organization { get; }

            public string FeedName { get; }

            public override string Name => $"AzureNPM:{Organization}/{FeedName}";

            protected override IDisposable TokenInjector( NPMPublishedProject project )
            {
                return project.TemporarySetPushTargetAndAzurePatLogin( FeedUri, ResolveAPIKey() );
            }
        }
    }
}
