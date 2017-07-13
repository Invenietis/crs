﻿using System.Threading.Tasks;

namespace CK.Crs
{

    public interface ICommandDispatcher
    {
        /// <summary>
        /// Sends the command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command to send.</param>
        /// <param name="context">The related <see cref="ICommandContext"/>.</param>
        /// <returns></returns>
        Task PostAsync<T>( T command, ICommandContext context ) where T : class;
    }
}
