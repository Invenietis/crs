using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class CommandRoutePath : IEquatable<CommandRoutePath>
    {
        const string PATH_SEPARATOR = "/";

        public string FullPath { get; set; }

        public string Prefix { get; set; }

        public string CommandName { get; set; }

        public CommandRoutePath( string requestPath )
        {
            FullPath = requestPath.ToLowerInvariant();

            int separatorIdx = requestPath.IndexOf( PATH_SEPARATOR );
            if( separatorIdx == -1 )
            {
                Prefix = String.Empty;
                CommandName = requestPath;
            }
            else
            {
                Prefix = requestPath.Substring( 0, separatorIdx ).ToLowerInvariant();
                CommandName = requestPath.Substring( separatorIdx ).ToLowerInvariant();
            }
        }

        internal bool IsValidFor( string commandReceiverPath )
        {
            return commandReceiverPath.ToLowerInvariant().Equals( Prefix );
        }

        public CommandRoutePath( string routePrefix, string commandName )
        {
            if( String.IsNullOrEmpty( routePrefix ) ) throw new ArgumentNullException( nameof( routePrefix ) );
            if( String.IsNullOrEmpty( commandName ) ) throw new ArgumentNullException( nameof( commandName ) );

            if( !routePrefix.StartsWith( PATH_SEPARATOR, StringComparison.InvariantCultureIgnoreCase ) ) throw new ArgumentException( $"The route prefix should start with a {PATH_SEPARATOR}" );

            Prefix = routePrefix.ToLowerInvariant();
            if( Prefix.EndsWith( PATH_SEPARATOR, StringComparison.InvariantCultureIgnoreCase ) ) Prefix = Prefix.Remove( Prefix.Length - 1 );

            CommandName = commandName.ToLowerInvariant();
            if( CommandName.StartsWith( PATH_SEPARATOR, StringComparison.InvariantCultureIgnoreCase ) ) CommandName = CommandName.Remove( 0, 1 );

            FullPath = $"{Prefix}{PATH_SEPARATOR}{CommandName}".ToLowerInvariant();
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
            var o = obj as CommandRoutePath;
            if( o != null ) return Equals( obj );

            return false;
        }

        public bool Equals( CommandRoutePath other )
        {
            return other.FullPath == this.FullPath;
        }

        public static implicit operator string( CommandRoutePath routePath )
        {
            return routePath.FullPath;
        }

        public static implicit operator CommandRoutePath( string routePath )
        {
            return new CommandRoutePath( routePath );
        }
    }
}
