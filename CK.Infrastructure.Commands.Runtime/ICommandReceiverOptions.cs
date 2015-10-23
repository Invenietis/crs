using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandReceiverOptions
    {
        /// <summary>
        /// Gets or sets the route prefix of this command receiver.
        /// </summary>
        string RoutePrefix { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="route"></param>
        /// <param name="isLongRunning"></param>
        /// <returns></returns>
        ICommandReceiverOptions Register<TCommand, THandler>( string route, bool isLongRunning )
            where TCommand : class
            where THandler : class, ICommandHandler;
    }
}
