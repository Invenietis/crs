using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Defines the a command route path: composed of a prefix (the receiver base path) and a command name.
    /// </summary>
    public struct CommandRoutePath : IEquatable<CommandRoutePath>
    {
        const string PATH_SEPARATOR = "/";

        /// <summary>
        /// Gets the full command route path
        /// </summary>
        public string FullPath { get; }

        /// <summary>
        /// Gets the command basePath
        /// </summary>
        public string BasePath { get; }

        /// <summary>
        /// Gets the command name
        /// </summary>
        public string CommandName { get; }

        /// <summary>
        /// Creates a <see cref="CommandRoutePath"/> from a full request path
        /// </summary>
        /// <param name="requestPath"></param>
        public CommandRoutePath( string requestPath )
        {
            FullPath = requestPath;

            int separatorIdx = requestPath.LastIndexOf( PATH_SEPARATOR );
            if( separatorIdx == -1 )
            {
                BasePath = String.Empty;
                CommandName = requestPath;
            }
            else
            {
                BasePath = requestPath.Substring( 0, separatorIdx );
                CommandName = requestPath.Substring( separatorIdx + 1 );
            }
        }

        /// <summary>
        /// Creates a <see cref="CommandRoutePath"/> from a basePath and a commandName.
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="commandName"></param>
        public CommandRoutePath( string basePath, string commandName )
        {
            if( String.IsNullOrEmpty( basePath ) ) throw new ArgumentNullException( nameof( basePath ) );
            if( String.IsNullOrEmpty( commandName ) ) throw new ArgumentNullException( nameof( commandName ) );

            if( !basePath.StartsWith( PATH_SEPARATOR, StringComparison.OrdinalIgnoreCase ) ) throw new ArgumentException( $"The route prefix should start with a {PATH_SEPARATOR}" );

            BasePath = EnsureNormalized( basePath );

            CommandName = commandName;
            if( CommandName.StartsWith( PATH_SEPARATOR, StringComparison.OrdinalIgnoreCase ) ) CommandName = CommandName.Remove( 0, 1 );

            FullPath = $"{BasePath}{PATH_SEPARATOR}{CommandName}";
        }

        /// <summary>
        /// Gets wether this instance is prefixed by the given receiver path
        /// </summary>
        /// <param name="receiverPath"></param>
        /// <returns></returns>
        public bool IsPrefixedBy( string receiverPath )
        {
            if( !receiverPath.StartsWith( PATH_SEPARATOR, StringComparison.OrdinalIgnoreCase ) )
                throw new ArgumentException( $"The receiverPath should start with a {PATH_SEPARATOR}" );

            receiverPath = EnsureNormalized( receiverPath );
            return receiverPath.Equals( BasePath, StringComparison.OrdinalIgnoreCase );
        }

        static string EnsureNormalized( string s )
        {
            if( s.EndsWith( PATH_SEPARATOR, StringComparison.OrdinalIgnoreCase ) ) return s.Remove( s.Length - 1 );
            return s;
        }

        public static string EnsureTrailingSlash( string receiverPath )
        {
            if( !String.IsNullOrWhiteSpace( receiverPath ) && 
                !receiverPath.EndsWith( PATH_SEPARATOR, StringComparison.OrdinalIgnoreCase ) )
                return String.Concat( receiverPath, '/' );

            return receiverPath;
        }

        public override string ToString()
        {
            return FullPath;
        }

        public override int GetHashCode()
        {
            return FullPath.ToLowerInvariant().GetHashCode();
        }

        public override bool Equals( object obj )
        {
            var o = (CommandRoutePath)obj;
            if( o != null ) return Equals( obj );

            return false;
        }

        public bool Equals( CommandRoutePath other )
        {
            return other.FullPath.Equals( this.FullPath, StringComparison.OrdinalIgnoreCase );
        }

        public static implicit operator string( CommandRoutePath routePath )
        {
            return routePath.FullPath;
        }

        public static implicit operator CommandRoutePath( string routePath )
        {
            return new CommandRoutePath( routePath );
        }

        public class Comparer : IEqualityComparer<CommandRoutePath>
        {
            public bool Equals( CommandRoutePath x, CommandRoutePath y )
            {
                return x.Equals( y );
            }

            public int GetHashCode( CommandRoutePath obj )
            {
                return obj.GetHashCode();
            }
        }
    }
}
