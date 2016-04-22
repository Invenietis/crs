using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CK.Crs
{
    public struct CommandRoutePath : IEquatable<CommandRoutePath>
    {
        const string PATH_SEPARATOR = "/";

        public string FullPath { get; set; }

        public string Prefix { get; set; }

        public string CommandName { get; set; }

        public CommandRoutePath( string requestPath )
        {
            FullPath = requestPath;

            int separatorIdx = requestPath.IndexOf( PATH_SEPARATOR );
            if( separatorIdx == -1 )
            {
                Prefix = String.Empty;
                CommandName = requestPath;
            }
            else
            {
                Prefix = requestPath.Substring( 0, separatorIdx );
                CommandName = requestPath.Substring( separatorIdx + 1 );
            }
        }

        public bool IsPrefixedBy( string receiverPath )
        {
            if( !receiverPath.StartsWith( PATH_SEPARATOR, StringComparison.OrdinalIgnoreCase ) )
                throw new ArgumentException( $"The receiverPath should start with a {PATH_SEPARATOR}" );

            receiverPath = EnsureNormalized( receiverPath );
            return receiverPath.Equals( Prefix, StringComparison.OrdinalIgnoreCase );
        }

        public CommandRoutePath( string routePrefix, string commandName )
        {
            if( String.IsNullOrEmpty( routePrefix ) ) throw new ArgumentNullException( nameof( routePrefix ) );
            if( String.IsNullOrEmpty( commandName ) ) throw new ArgumentNullException( nameof( commandName ) );

            if( !routePrefix.StartsWith( PATH_SEPARATOR, StringComparison.OrdinalIgnoreCase ) ) throw new ArgumentException( $"The route prefix should start with a {PATH_SEPARATOR}" );

            Prefix = EnsureNormalized( routePrefix );

            CommandName = commandName;
            if( CommandName.StartsWith( PATH_SEPARATOR, StringComparison.OrdinalIgnoreCase ) ) CommandName = CommandName.Remove( 0, 1 );

            FullPath = $"{Prefix}{PATH_SEPARATOR}{CommandName}";
        }

        static string EnsureNormalized( string s )
        {
            if( s.EndsWith( PATH_SEPARATOR, StringComparison.OrdinalIgnoreCase ) ) return s.Remove( s.Length - 1 );
            return s;
        }

        public override string ToString()
        {
            return FullPath;
        }

        public override int GetHashCode()
        {
            return FullPath.GetHashCode();
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
