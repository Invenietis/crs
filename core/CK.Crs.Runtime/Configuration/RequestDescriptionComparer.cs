using System.Collections.Generic;

namespace CK.Crs.Infrastructure
{
    internal class RequestDescriptionComparer : IEqualityComparer<CommandModel>
    {
        public bool Equals( CommandModel x, CommandModel y )
        {
            if( x == null || y == null ) return false;

            return x.CommandType == y.CommandType && x.HandlerType == y.HandlerType;
        }

        public int GetHashCode( CommandModel obj )
        {
            if( obj == null ) return 0;

            return obj.CommandType.GetHashCode() ^ obj.HandlerType.GetHashCode();
        }
    }
}
