using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface IActorIdProvider
    {
        /// <summary>
        /// Gets user identifier by name
        /// </summary>
        /// <param name="userName">A user name</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The user identifier (<see cref="int"/>).</returns>
        Task<int> GetUserIdAsync( string userName, CancellationToken cancellationToken = default( CancellationToken ) );
    }

    public interface IUserNameProvider
    {
        /// <summary>
        /// Gets the user name of the authenticated user.
        /// </summary>
        /// <returns>The user name.</returns>
        Task<string> GetAuthenticatedUserNameAsync();
    }

    public class ActorIdProvider : IAmbientValueProvider
    {
        readonly IActorIdProvider _actorIdProvider;
        readonly IUserNameProvider _userNameProvider;

        public ActorIdProvider( IActorIdProvider actorIdProvider, IUserNameProvider userNameProvider )
        {
            if( actorIdProvider == null ) throw new ArgumentNullException( nameof( actorIdProvider ), "You must provides an implementation of IActorIdProvider." );
            if( userNameProvider == null ) throw new ArgumentNullException( nameof( userNameProvider ), "You must provides an implementation of IUserNameProvider." );

            _actorIdProvider = actorIdProvider;
            _userNameProvider = userNameProvider;
        }

        public const int Default = 0;

        public async Task<object> GetValueAsync( IAmbientValues values )
        {
            var userName = await _userNameProvider.GetAuthenticatedUserNameAsync();
            if( String.IsNullOrEmpty( userName ) ) return Default;

            UserIdResult result = await PreReadUserIdAsync( userName, values );
            if( result == null ) throw new InvalidOperationException( "PreReadUserIdAsync must return a non-null UserIdResult." );
            if( result.IsValid ) return result.UserId;

            // Provider call.
            result.UserId = await _actorIdProvider.GetUserIdAsync( userName );

            result = await PostReadUserIdAsync( result.UserId, userName, values );
            if( result == null ) throw new InvalidOperationException( "PostReadUserIdAsync must return a non-null UserIdResult." );
            if( result.IsValid ) return result.UserId;

            return Default;
        }

        protected class UserIdResult
        {
            public int UserId { get; set; }

            public bool IsValid { get; set; }
        }

        protected virtual Task<UserIdResult> PreReadUserIdAsync( string userName, IAmbientValues values )
        {
            return Task.FromResult( new UserIdResult { UserId = 0, IsValid = false } );
        }

        protected virtual Task<UserIdResult> PostReadUserIdAsync( int userId, string userName, IAmbientValues values )
        {
            return Task.FromResult( new UserIdResult { UserId = userId, IsValid = true } );
        }
    }
}
