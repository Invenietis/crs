using System.Collections.Generic;

namespace CK.Crs.Infrastructure
{
    internal class CommandModelComparer : IEqualityComparer<CommandModel>
    {
        public bool Equals( CommandModel x, CommandModel y )
        {
            if( x == null || y == null ) return false;

            return x.CommandType == y.CommandType;
        }

        public int GetHashCode( CommandModel obj )
        {
            if( obj == null ) return 0;

            return obj.CommandType.GetHashCode();
        }
    }
}
