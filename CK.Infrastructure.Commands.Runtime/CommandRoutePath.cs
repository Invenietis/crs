using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class CommandRoutePath : IEquatable<CommandRoutePath>
    {
        private static readonly Regex DefaultRoute = new Regex( @"\/(\w+)\/(\w+)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        public string Value { get; set; }

        public CommandRoutePath( string requestPath )
        {
            Value = requestPath.ToLowerInvariant();
        }

        public CommandRoutePath( string routePrefix, string commandTypeName )
        {
            Value = String.Concat( routePrefix, "/", commandTypeName ).ToLowerInvariant();
        }

        public override string ToString()
        {
            return Value;
        }

        public string ExtractCommandClassName()
        {
            if( !DefaultRoute.IsMatch( Value ) ) return null;

            return DefaultRoute.Match( Value ).Groups[3].Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            var o = obj as CommandRoutePath;
            if( o != null ) return Equals( obj );

            return false;
        }

        public bool Equals( CommandRoutePath other )
        {
            return other.Value == this.Value;
        }

        public bool IsValid( string routePrefix )
        {
            return Value.StartsWith( routePrefix ) && DefaultRoute.IsMatch( Value );
        }
    }
}
