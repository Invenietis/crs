using System;

namespace CK.Crs
{
    public interface ICommandHandlerActivator
    {
        object Create( Type t );

        void Release( object o );
    }
}
