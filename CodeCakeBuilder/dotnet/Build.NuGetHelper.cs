using Cake.Common.Diagnostics;
using Cake.Core;
using CK.Text;
using CodeCake.Abstractions;
using CSemVer;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Credentials;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol.Plugins;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeCake
{
    public partial class Build
    {
        public static class NuGetHelper
        {
            static readonly SourceCacheContext _sourceCache;
            static readonly List<Lazy<INuGetResourceProvider>> _providers;
            static readonly ISettings _settings;
            static readonly PackageProviderProxy _sourceProvider;
            static readonly List<VSTSFeed> _vstsFeeds;
            static ILogger _logger;

            /// <summary>
            /// Implements a IPackageSourceProvider that mixes sources from NuGet.config settings
            /// and sources that are used by the build chain.
            /// </summary>
            class PackageProviderProxy : IPackageSourceProvider
            {
                readonly IPackageSourceProvider _fromSettings;
                readonly Lazy<List<PackageSource>> _sources;
                int _definedSourceCount;

                public PackageProviderProxy( ISettings settings )
                {
                    _fromSettings = new PackageSourceProvider( settings );
                    _sources = new Lazy<List<PackageSource>>( () => new List<PackageSource>( _fromSettings.LoadPackageSources() ) );
                }

                public PackageSource FindOrCreateFromUrl( string name, string urlV3 )
                {
                    if( string.IsNullOrEmpty( urlV3 ) || (!new Uri( urlV3 ).IsFile && !urlV3.EndsWith( "/v3/index.json" )) )
                    {
                        throw new ArgumentException( "Feed requires a /v3/index.json url.", nameof( urlV3 ) );
                    }
                    if( string.IsNullOrWhiteSpace( name ) )
                    {
                        throw new ArgumentNullException( nameof( name ) );
                    }
                    var exists = _sources.Value.FirstOrDefault( s => !s.IsLocal && s.Source == urlV3 );
                    if( exists != null ) return exists;
                    exists = new PackageSource( urlV3, "CCB-" + name );
                    _sources.Value.Insert( _definedSourceCount++, exists );
                    return exists;
                }

                public PackageSource FindOrCreateFromLocalPath( string localPath )
                {
                    if( string.IsNullOrWhiteSpace( localPath ) ) throw new ArgumentNullException( nameof( localPath ) );
                    NormalizedPath path = System.IO.Path.GetFullPath( localPath );
                    var exists = _sources.Value.FirstOrDefault( s => s.IsLocal && new NormalizedPath( s.Source ) == path );
                    if( exists != null ) return exists;
                    exists = new PackageSource( path, "CCB-" + path.LastPart );
                    _sources.Value.Insert( _definedSourceCount++, exists );
                    return exists;
                }

                string IPackageSourceProvider.ActivePackageSourceName => _fromSettings.ActivePackageSourceName;

                string IPackageSourceProvider.DefaultPushSource => _fromSettings.DefaultPushSource;

                event EventHandler IPackageSourceProvider.PackageSourcesChanged { add { } remove { } }

                /// <summary>
                /// Gets all the sources.
                /// </summary>
                /// <returns></returns>
                public IEnumerable<PackageSource> LoadPackageSources() => _sources.Value;

                bool IPackageSourceProvider.IsPackageSourceEnabled( string name ) => true;

                void IPackageSourceProvider.SaveActivePackageSource( PackageSource source )
                {
                    throw new NotSupportedException( "Should not be called in this scenario." );
                }

                void IPackageSourceProvider.SavePackageSources( IEnumerable<PackageSource> sources )
                {
                    throw new NotSupportedException( "Should not be called in this scenario." );
                }

                PackageSource IPackageSourceProvider.GetPackageSourceByName( string name ) => _sources.Value.FirstOrDefault( s => s.Name == name );

                PackageSource IPackageSourceProvider.GetPackageSourceBySource( string source ) => _sources.Value.FirstOrDefault( s => s.Source == source );

                void IPackageSourceProvider.RemovePackageSource( string name )
                {
                    throw new NotSupportedException( "Should not be called in this scenario." );
                }

                void IPackageSourceProvider.EnablePackageSource( string name )
                {
                    throw new NotSupportedException( "Should not be called in this scenario." );
                }

                void IPackageSourceProvider.DisablePackageSource( string name )
                {
                    throw new NotSupportedException( "Should not be called in this scenario." );
                }

                void IPackageSourceProvider.UpdatePackageSource( PackageSource source, bool updateCredentials, bool updateEnabled )
                {
                    throw new NotSupportedException( "Should not be called in this scenario." );
                }

                void IPackageSourceProvider.AddPackageSource( PackageSource source )
                {
                    throw new NotSupportedException( "Should not be called in this scenario." );
                }
            }

            static NuGetHelper()
            {
                _settings = Settings.LoadDefaultSettings( Environment.CurrentDirectory );
                _sourceProvider = new PackageProviderProxy( _settings );
                _vstsFeeds = new List<VSTSFeed>();
                // Setting "NoCache" (?) here is required to be able to retry a push after a
                // failure. Without it, the PUT is canceled.
                _sourceCache = new SourceCacheContext().WithRefreshCacheTrue();
                _providers = new List<Lazy<INuGetResourceProvider>>();
                _providers.AddRange( Repository.Provider.GetCoreV3() );
            }

            class Logger : ILogger
            {
                readonly ICakeContext _ctx;
                readonly object _lock;

                public Logger( ICakeContext ctx )
                {
                    _ctx = ctx;
                    _lock = new object();
                }

                public void LogDebug( string data ) { lock( _lock ) _ctx.Debug( $"NuGet: {data}" ); }
                public void LogVerbose( string data ) { lock( _lock ) _ctx.Verbose( $"NuGet: {data}" ); }
                public void LogInformation( string data ) { lock( _lock ) _ctx.Information( $"NuGet: {data}" ); }
                public void LogMinimal( string data ) { lock( _lock ) _ctx.Information( $"NuGet: {data}" ); }
                public void LogWarning( string data ) { lock( _lock ) _ctx.Warning( $"NuGet: {data}" ); }
                public void LogError( string data ) { lock( _lock ) _ctx.Error( $"NuGet: {data}" ); }
                public void LogSummary( string data ) { lock( _lock ) _ctx.Information( $"NuGet: {data}" ); }
                public void LogInformationSummary( string data ) { lock( _lock ) _ctx.Information( $"NuGet: {data}" ); }
                public void Log( LogLevel level, string data ) { lock( _lock ) _ctx.Information( $"NuGet ({level}): {data}" ); }
                public Task LogAsync( LogLevel level, string data )
                {
                    Log( level, data );
                    return System.Threading.Tasks.Task.CompletedTask;
                }

                public void Log( ILogMessage message )
                {
                    lock( _lock ) _ctx.Information( $"NuGet ({message.Level}) - Code: {message.Code} - Project: {message.ProjectPath} - {message.Message}" );
                }

                public Task LogAsync( ILogMessage message )
                {
                    Log( message );
                    return System.Threading.Tasks.Task.CompletedTask;
                }
            }

            static ILogger InitializeAndGetLogger( ICakeContext ctx )
            {
                if( _logger == null )
                {
                    ctx.Information( $"Initializing with sources:" );
                    foreach( var s in _sourceProvider.LoadPackageSources() )
                    {
                        ctx.Information( $"{s.Name} => {s.Source}" );
                    }
                    _logger = new Logger( ctx );

                }
                return _logger;
            }

            class Creds : ICredentialProvider
            {
                private readonly ICakeContext _ctx;

                public Creds( ICakeContext ctx )
                {
                    _ctx = ctx;
                }

                public string Id { get; }

                public Task<CredentialResponse> GetAsync(
                    Uri uri,
                    IWebProxy proxy,
                    CredentialRequestType type,
                    string message,
                    bool isRetry,
                    bool nonInteractive,
                    CancellationToken cancellationToken ) =>
                    System.Threading.Tasks.Task.FromResult(
                        new CredentialResponse(
                            new NetworkCredential(
                                "CKli",
                                _ctx.InteractiveEnvironmentVariable(
                                    _vstsFeeds.Single( p => new Uri( p.Url ).ToString() == uri.ToString() ).SecretKeyName
                                )
                            )
                        )
                    );
            }

            /// <summary>
            /// Base class for NuGet feeds.
            /// </summary>
            public abstract class NuGetFeed : ArtifactFeed
            {
                readonly PackageSource _packageSource;
                readonly SourceRepository _sourceRepository;
                readonly AsyncLazy<PackageUpdateResource> _updater;
                /// <summary>
                /// Initialize a new remote feed.
                /// Its final <see cref="Name"/> is the one of the existing feed if it appears in the existing
                /// sources (from NuGet configuration files) or "CCB-<paramref name="name"/>" if this is
                /// an unexisting source (CCB is for CodeCakeBuilder). 
                /// </summary>
                /// <param name="type">The central NuGet handler.</param>
                /// <param name="name">Name of the feed.</param>
                /// <param name="urlV3">Must be a v3/index.json url otherwise an argument exception is thrown.</param>
                protected NuGetFeed( NuGetArtifactType type, string name, string urlV3 )
                    : this( type, _sourceProvider.FindOrCreateFromUrl( name, urlV3 ) )
                {
                    if( this is VSTSFeed f )
                    {
                        if( HttpHandlerResourceV3.CredentialService == null )
                        {
                            HttpHandlerResourceV3.CredentialService = new Lazy<ICredentialService>(
                            () => new CredentialService(
                                providers: new AsyncLazy<IEnumerable<ICredentialProvider>>(
                                    () => System.Threading.Tasks.Task.FromResult<IEnumerable<ICredentialProvider>>(
                                        new List<Creds> { new Creds( Cake ) } )
                                ),
                                nonInteractive: true,
                                handlesDefaultCredentials: true )
                            );
                        }
                        _vstsFeeds.Add( f );
                    }
                }

                /// <summary>
                /// Initialize a new local feed.
                /// Its final <see cref="Name"/> is the one of the existing feed if it appears in the existing
                /// sources (from NuGet configuration files) or "CCB-GetDirectoryName(localPath)" if this is
                /// an unexisting source (CCB is for CodeCakeBuilder). 
                /// </summary>
                /// <param name="type">The central NuGet handler.</param>
                /// <param name="localPath">Local path.</param>
                protected NuGetFeed( NuGetArtifactType type, string localPath )
                    : this( type, _sourceProvider.FindOrCreateFromLocalPath( localPath ) )
                {
                }

                NuGetFeed( NuGetArtifactType type, PackageSource s )
                    : base( type )
                {
                    _packageSource = s;
                    _sourceRepository = new SourceRepository( _packageSource, _providers );
                    _updater = new AsyncLazy<PackageUpdateResource>( async () =>
                    {
                        var r = await _sourceRepository.GetResourceAsync<PackageUpdateResource>();
                        // TODO: Update for next NuGet version?
                        // r.Settings = _settings;
                        return r;
                    } );
                }

                /// <summary>
                /// Must provide the secret key name that must be found in the environment variables.
                /// Without it push is skipped.
                /// </summary>
                public abstract string SecretKeyName { get; }

                /// <summary>
                /// The url of the source. Can be a local path.
                /// </summary>
                public string Url => _packageSource.Source;

                /// <summary>
                /// Gets whether this is a local feed (a directory).
                /// </summary>
                public bool IsLocal => _packageSource.IsLocal;

                /// <summary>
                /// Gets the source name.
                /// If the source appears in NuGet configuration files, it is the configured name of the source, otherwise
                /// it is prefixed with "CCB-" (CCB is for CodeCakeBuilder). 
                /// </summary>
                public override string Name => _packageSource.Name;

                /// <summary>
                /// Creates a list of push entries from a set of local artifacts into this feed.
                /// </summary>
                /// <param name="artifacts">Local artifacts.</param>
                /// <returns>The set of push into this feed.</returns>
                public override async Task<IEnumerable<ArtifactPush>> CreatePushListAsync( IEnumerable<ILocalArtifact> artifacts )
                {
                    var result = new List<ArtifactPush>();
                    var logger = InitializeAndGetLogger( Cake );
                    MetadataResource meta = await _sourceRepository.GetResourceAsync<MetadataResource>();
                    foreach( var p in artifacts )
                    {
                        var pId = new PackageIdentity( p.ArtifactInstance.Artifact.Name, new NuGetVersion( p.ArtifactInstance.Version.ToNormalizedString() ) );
                        if( await meta.Exists( pId, _sourceCache, logger, CancellationToken.None ) )
                        {
                            Cake.Debug( $" ==> Feed '{Name}' already contains {p.ArtifactInstance}." );
                        }
                        else
                        {
                            Cake.Debug( $"Package {p.ArtifactInstance} must be published to remote feed '{Name}'." );
                            result.Add( new ArtifactPush( p, this ) );
                        }
                    }
                    return result;
                }

                /// <summary>
                /// Pushes a set of packages from .nupkg files that must exist in <see cref="CheckRepositoryInfo.ReleasesFolder"/>.
                /// </summary>
                /// <param name="pushes">The instances to push (that necessary target this feed).</param>
                /// <returns>The awaitable.</returns>
                public override async Task PushAsync( IEnumerable<ArtifactPush> pushes )
                {
                    string apiKey = null;
                    if( !_packageSource.IsLocal )
                    {
                        apiKey = ResolveAPIKey();
                        if( string.IsNullOrEmpty( apiKey ) )
                        {
                            Cake.Information( $"Could not resolve API key. Push to '{Name}' => '{Url}' is skipped." );
                            return;
                        }
                    }
                    Cake.Information( $"Pushing packages to '{Name}' => '{Url}'." );
                    var logger = InitializeAndGetLogger( Cake );
                    var updater = await _updater;
                    foreach( var p in pushes )
                    {
                        string packageString = p.Name + "." + p.Version.WithBuildMetaData( null ).ToNormalizedString();
                        var fullPath = ArtifactType.GlobalInfo.ReleasesFolder.AppendPart( packageString + ".nupkg" );
                        await updater.Push(
                            fullPath,
                            string.Empty, // no Symbol source.
                            20, //20 seconds timeout
                            disableBuffering: false,
                            getApiKey: endpoint => apiKey,
                            getSymbolApiKey: symbolsEndpoint => null,
                            noServiceEndpoint: false,
                            skipDuplicate: true,
                            symbolPackageUpdateResource: null,
                            log: logger );
                    }
                    await OnAllArtifactsPushed( pushes );
                }

                /// <summary>
                /// Called once all the packages are pushed.
                /// Does nothing at this level.
                /// </summary>
                /// <param name="pushes">The instances to push (that necessary target this feed).</param>
                /// <returns>The awaitable.</returns>
                protected virtual Task OnAllArtifactsPushed( IEnumerable<ArtifactPush> pushes )
                {
                    return System.Threading.Tasks.Task.CompletedTask;
                }

                /// <summary>
                /// Must resolve the API key required to push the package.
                /// </summary>
                /// <returns>The secret (that can be null or empty).</returns>
                protected abstract string ResolveAPIKey();
            }
        }

        /// <summary>
        /// A basic VSTS feed uses "VSTS" for the API key and does not handle views.
        /// The https://github.com/Microsoft/artifacts-credprovider must be installed.
        /// A Personal Access Token, <see cref="SecretKeyName"/> environment variable
        /// must be defined and contains the token.
        /// If this SecretKeyName is not defined or empty, push is skipped.
        /// </summary>
        class VSTSFeed : NuGetHelper.NuGetFeed
        {
            string _azureFeedPAT;

            /// <summary>
            /// Initialize a new remote VSTS feed.
            /// </summary>
            /// <param name="name">Name of the feed.</param>
            /// <param name="urlV3">Must be a v3/index.json url otherwise an argument exception is thrown.</param>
            /// <param name="secretKeyName">The secret key name. When null or empty, push is skipped.</param>
            public VSTSFeed( NuGetArtifactType t, string name, string urlV3, string secretKeyName )
                : base( t, name, urlV3 )
            {
                SecretKeyName = secretKeyName;
            }

            /// <summary>
            /// Gets the name of the environment variable that must contain the
            /// Personal Access Token that allows push to this feed.
            /// The  https://github.com/Microsoft/artifacts-credprovider VSS_NUGET_EXTERNAL_FEED_ENDPOINTS
            /// will be dynalically generated.
            /// </summary>
            public override string SecretKeyName { get; }

            /// <summary>
            /// Looks up for the <see cref="SecretKeyName"/> environment variable that is required to promote packages.
            /// If this variable is empty or not defined, push is skipped.
            /// </summary>
            /// <param name="ctx">The Cake context.</param>
            /// <returns>The "VSTS" API key or null to skip the push.</returns>
            protected override string ResolveAPIKey()
            {
                _azureFeedPAT = Cake.InteractiveEnvironmentVariable( SecretKeyName );
                if( string.IsNullOrWhiteSpace( _azureFeedPAT ) )
                {
                    Cake.Warning( $"No {SecretKeyName} environment variable found." );
                    _azureFeedPAT = null;
                }
                // The API key for the Credential Provider must be "VSTS".
                return _azureFeedPAT != null ? "VSTS" : null;
            }
        }

        /// <summary>
        /// A SignatureVSTSFeed handles Stable, Latest, Preview and CI Azure feed views with
        /// package promotion based on the published version.
        /// The secret key name is built by <see cref="GetSecretKeyName"/>:
        /// "AZURE_FEED_" + Organization.ToUpperInvariant().Replace( '-', '_' ).Replace( ' ', '_' ) + "_PAT".
        /// </summary>
        class SignatureVSTSFeed : VSTSFeed
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

            /// <summary>
            /// Initialize a new SignatureVSTSFeed.
            /// Its <see cref="NuGetHelper.Feed.Name"/> is set to "<paramref name="organization"/>-<paramref name="feedName"/>"
            /// (and may be prefixed with "CCB-" if it doesn't correspond to a source defined in the NuGet.config settings.
            /// </summary>
            /// <param name="organization">Name of the organization.</param>
            /// <param name="feedName">Identifier of the feed in Azure, inside the organization.</param>
            public SignatureVSTSFeed( NuGetArtifactType t, string organization, string feedName, string projectName )
                : base( t, organization + "-" + feedName,
                      projectName != null ?
                          $"https://pkgs.dev.azure.com/{organization}/{projectName}/_packaging/{feedName}/nuget/v3/index.json"
                        : $"https://pkgs.dev.azure.com/{organization}/_packaging/{feedName}/nuget/v3/index.json",
                        GetSecretKeyName( organization ) )
            {
                Organization = organization;
                FeedName = feedName;
                ProjectName = projectName;
            }

            /// <summary>
            /// Gets the organization name.
            /// </summary>
            public string Organization { get; }

            /// <summary>
            /// Gets the feed identifier.
            /// </summary>
            public string FeedName { get; }

            public string ProjectName { get; }

            /// <summary>
            /// Implements Package promotion in @CI, @Exploratory, @Preview, @Latest and @Stable views.
            /// </summary>
            /// <param name="ctx">The Cake context.</param>
            /// <param name="pushes">The set of artifacts to promote.</param>
            /// <returns>The awaitable.</returns>
            protected override async Task OnAllArtifactsPushed( IEnumerable<ArtifactPush> pushes )
            {
                var basicAuth = Convert.ToBase64String( Encoding.ASCII.GetBytes( ":" + Cake.InteractiveEnvironmentVariable( SecretKeyName ) ) );
                foreach( var p in pushes )
                {
                    foreach( var view in p.Version.PackageQuality.GetLabels() )
                    {
                        var url = ProjectName != null ?
                              $"https://pkgs.dev.azure.com/{Organization}/{ProjectName}/_apis/packaging/feeds/{FeedName}/nuget/packagesBatch?api-version=5.0-preview.1"
                            : $"https://pkgs.dev.azure.com/{Organization}/_apis/packaging/feeds/{FeedName}/nuget/packagesBatch?api-version=5.0-preview.1";
                        using( HttpRequestMessage req = new HttpRequestMessage( HttpMethod.Post, url ) )
                        {
                            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue( "Basic", basicAuth );
                            var body = GetPromotionJSONBody( p.Name, p.Version.ToNormalizedString(), view.ToString() );
                            req.Content = new StringContent( body, Encoding.UTF8, "application/json" );
                            using( var m = await StandardGlobalInfo.SharedHttpClient.SendAsync( req ) )
                            {
                                if( m.IsSuccessStatusCode )
                                {
                                    Cake.Information( $"Package '{p.Name}' promoted to view '@{view}'." );
                                }
                                else
                                {
                                    Cake.Error( $"Package '{p.Name}' promotion to view '@{view}' failed." );
                                    // Throws!
                                    m.EnsureSuccessStatusCode();
                                }
                            }
                        }
                    }
                }
            }

            string GetPromotionJSONBody( string packageName, string packageVersion, string viewId, bool npm = false )
            {
                var bodyFormat = @"{
 ""data"": {
    ""viewId"": ""{viewId}""
  },
  ""operation"": 0,
  ""packages"": [{
    ""id"": ""{packageName}"",
    ""version"": ""{packageVersion}"",
    ""protocolType"": ""{NuGetOrNpm}""
  }]
}";
                return bodyFormat.Replace( "{NuGetOrNpm}", npm ? "Npm" : "NuGet" )
                                 .Replace( "{viewId}", viewId )
                                 .Replace( "{packageName}", packageName )
                                 .Replace( "{packageVersion}", packageVersion );
            }

        }

        /// <summary>
        /// A remote feed where push is controlled by its <see cref="SecretKeyName"/>.
        /// </summary>
        class RemoteFeed : NuGetHelper.NuGetFeed
        {
            /// <summary>
            /// Initialize a new remote feed.
            /// The push is controlled by an API key name that is the name of an environment variable
            /// that must contain the actual API key to push packages.
            /// </summary>
            /// <param name="name">Name of the feed.</param>
            /// <param name="urlV3">Must be a v3/index.json url otherwise an argument exception is thrown.</param>
            /// <param name="secretKeyName">The secret key name.</param>
            public RemoteFeed( NuGetArtifactType t, string name, string urlV3, string secretKeyName )
                : base( t, name, urlV3 )
            {
                SecretKeyName = secretKeyName;
            }

            /// <summary>
            /// Gets or sets the push API key name.
            /// This is the environment variable name that must contain the NuGet API key required to push.
            /// </summary>
            public override string SecretKeyName { get; }

            /// <summary>
            /// Resolves the API key from <see cref="APIKeyName"/> environment variable.
            /// </summary>
            /// <param name="ctx">The Cake context.</param>
            /// <returns>The API key or null.</returns>
            protected override string ResolveAPIKey()
            {
                if( String.IsNullOrEmpty( SecretKeyName ) )
                {
                    Cake.Information( $"Remote feed '{Name}' APIKeyName is null or empty." );
                    return null;
                }
                return Cake.InteractiveEnvironmentVariable( SecretKeyName );
            }
        }

        /// <summary>
        /// Local feed. Pushes are always possible.
        /// </summary>
        class NugetLocalFeed : NuGetHelper.NuGetFeed
        {
            public NugetLocalFeed( NuGetArtifactType t, string path )
                : base( t, path )
            {
            }

            public override string SecretKeyName => null;

            protected override string ResolveAPIKey() => null;
        }
    }
}

