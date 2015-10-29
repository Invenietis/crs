using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{

    public static class CommandReceiverExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="route"></param>
        /// <param name="isLongRunning"></param>
        /// <returns></returns>
        public static CommandReceiverOptions Register<TCommand, THandler>( this CommandReceiverOptions o, string route, bool isLongRunning )
            where TCommand : class
            where THandler : class, ICommandHandler<TCommand>
        {
            o.Registry.Register( new CommandDescriptor
            {
                CommandType = typeof( TCommand ),
                Route = route,
                IsLongRunning = isLongRunning,
                HandlerType = typeof( THandler )
            } );

            return o;
        }
    }
}