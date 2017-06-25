﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Execution
{
    internal interface IExecutionFactory
    {
        /// <summary>
        /// Creates an instance of the given type or return null if the type cannot be instanciated.
        /// </summary>
        /// <param name="decoratorType">The type of the decorator to create</param>
        /// <returns>An instance of <see cref="ICommandDecorator"/> or null.</returns>
        ICommandDecorator CreateDecorator( Type decoratorType );

        /// <summary>
        /// Creates an instance of the given type or return null if the type cannot be instanciated.
        /// </summary>
        /// <param name="handlerType">The type of the handler to create.</param>
        /// <returns></returns>
        ICommandHandler CreateHandler( Type handlerType );

        /// <summary>
        /// Releases a <see cref="ICommandHandler"/>.
        /// </summary>
        /// <param name="handler">The command handler to release.</param>
        void ReleaseHandler( ICommandHandler handler );
    }
}
