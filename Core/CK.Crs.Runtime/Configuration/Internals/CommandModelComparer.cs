using System.Collections.Generic;

namespace CK.Crs.Configuration
{
    internal class CommandModelComparer : IEqualityComparer<ICommandModel>
    {
        public bool Equals( ICommandModel x, ICommandModel y )
        {
            if( x == null || y == null ) return false;

            return x.CommandType == y.CommandType;
        }

        public int GetHashCode( ICommandModel obj )
        {
            if( obj == null ) return 0;

            return obj.CommandType.GetHashCode();
        }
    }
}
