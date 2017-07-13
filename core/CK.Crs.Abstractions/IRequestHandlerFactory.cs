using System;

namespace CK.Crs
{
    public interface IRequestHandlerFactory
    {
        IRequestHandler CreateHandler( Type handlerType );

        void ReleaseHandler( IRequestHandler handler );
    }
}
