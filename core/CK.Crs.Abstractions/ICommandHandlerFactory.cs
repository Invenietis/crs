using System;

namespace CK.Crs
{
    public interface ICommandHandlerFactory
    {
        ICommandHandler CreateHandler( Type handlerType );

        void ReleaseHandler( ICommandHandler handler );
    }
}
