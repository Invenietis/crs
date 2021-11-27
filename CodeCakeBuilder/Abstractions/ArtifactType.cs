using Cake.Common.Diagnostics;
using Cake.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCake.Abstractions
{
    /// <summary>
    /// Abstract artifact Type that handles <see cref="ArtifactFeed"/> and defines
    /// how <see cref="ILocalArtifact"/> can be published into those feeds.
    /// </summary>
    public abstract class ArtifactType
    {
        readonly StandardGlobalInfo _globalInfo;
        readonly string _typeName;
        List<ArtifactFeed>? _feeds;
        List<ILocalArtifact>? _artifacts;
        List<ArtifactPush>? _pushes;

        /// <summary>
        /// Initializes a new artifact type and adds it into
        /// the <paramref name="globalInfo"/>.
        /// </summary>
        /// <param name="globalInfo">The global object.</param>
        /// <param name="typeName">Name of the type.</param>
        protected ArtifactType( StandardGlobalInfo globalInfo, string typeName )
        {
            _globalInfo = globalInfo;
            _typeName = typeName;
        }

        /// <summary>
        /// Gets the artifact type name.
        /// </summary>
        public string TypeName => _typeName;

        /// <summary>
        /// Gets the global <see cref="CheckRepositoryInfo"/>.
        /// </summary>
        public StandardGlobalInfo GlobalInfo => _globalInfo;

        /// <summary>
        /// Gets a mutable list of all the target feeds into which artifacts of this type should be pushed.
        /// </summary>
        /// <returns>The list of feeds.</returns>
        public IList<ArtifactFeed> GetTargetFeeds( bool reset = false )
        {
            if( _feeds == null || reset )
            {
                _feeds = new List<ArtifactFeed>();
                if( GlobalInfo.LocalFeedPath != null )
                {
                    foreach( var f in GetLocalFeeds() )
                    {
                        GlobalInfo.Cake.Information( $"Adding local feed {f.Name}." );
                        if( f.ArtifactType != this )
                        {
                            throw new InvalidOperationException( $"Feed type mismatch." );
                        }
                        _feeds.Add( f );
                    }
                }
                if( GlobalInfo.PushToRemote )
                {
                    foreach( ArtifactFeed f in GetRemoteFeeds() )
                    {
                        GlobalInfo.Cake.Information( $"Adding remote feed: {f.Name}" );
                        if( f.ArtifactType != this )
                        {
                            throw new InvalidOperationException( $"Feed type mismatch." );
                        }
                        _feeds.Add( f );
                    }
                }
            }
            return _feeds;
        }

        /// <summary>
        /// Gets a mutable list of all locally produced artifacts of this type.
        /// </summary>
        /// <param name="reset">True to recompute a list.</param>
        /// <returns>The set of artifacts.</returns>
        public IList<ILocalArtifact> GetArtifacts( bool reset = false )
        {
            if( _artifacts == null || reset )
            {
                _artifacts = new List<ILocalArtifact>();
                foreach( var a in GetLocalArtifacts() )
                {
                    if( a.ArtifactInstance.Artifact.Type != _typeName )
                    {
                        throw new InvalidOperationException( $"Artifact type mismatch: expected '{_typeName}' but got '{a.ArtifactInstance.Artifact.Type}'." );
                    }
                    _artifacts.Add( a );
                }
            }
            return _artifacts;
        }

        /// <summary>
        /// Gets a mutable list of all the pushes of artifacts into target feeds for this type.
        /// </summary>
        /// <returns>The set of pushes.</returns>
        public async Task<IList<ArtifactPush>> GetPushListAsync( bool reset = false )
        {
            if( _pushes == null || reset )
            {
                _pushes = new List<ArtifactPush>();
                var locals = GetArtifacts();
                var tasks = GetTargetFeeds().Select( f => f.CreatePushListAsync( locals ) ).ToArray();
                foreach( var p in await Task.WhenAll( tasks ) )
                {
                    _pushes.AddRange( p );
                }
            }
            return _pushes;
        }

        /// <summary>
        /// Must push all the required artifacts into all the target feeds.
        /// This uses the <see cref="GetPushListAsync(bool)"/> by default.
        /// </summary>
        /// <param name="pushes">Push details: defaults to the result of <see cref="GetPushListAsync"/>.</param>
        public async Task PushAsync( IEnumerable<ArtifactPush>? pushes = null )
        {
            if( pushes == null ) pushes = await GetPushListAsync();
            var tasks = pushes.GroupBy( p => p.Feed ).Select( g => g.Key.PushAsync( g ) ).ToArray();
            await Task.WhenAll( tasks );
        }

        /// <summary>
        /// Must get the remote target feeds into which artifacts of this
        /// type should be pushed.
        /// </summary>
        /// <returns>A set of remote feed.</returns>
        protected abstract IEnumerable<ArtifactFeed> GetRemoteFeeds();

        /// <summary>
        /// Must get the local target feeds into which artifacts of this
        /// type should be pushed.
        /// </summary>
        /// <returns>A set of local feeds.</returns>
        protected abstract IEnumerable<ArtifactFeed> GetLocalFeeds();

        /// <summary>
        /// Must get the locally produced artifacts of this type.
        /// </summary>
        /// <returns>A set of local artifacts.</returns>
        protected abstract IEnumerable<ILocalArtifact> GetLocalArtifacts();

    }
}
