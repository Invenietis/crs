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
        /// Gets the command registry to register command and handlers.
        /// </summary>
        ICommandRegistry Registry { get; }
    }

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
        public static ICommandReceiverOptions Register<TCommand, THandler>( this ICommandReceiverOptions o, string route, bool isLongRunning ) 
            where TCommand : class
            where THandler : class, ICommandHandler<TCommand>
        {
            o.Registry.Register( new CommandDescriptor
            {
                CommandType = typeof( TCommand ),
                Route = new CommandRoutePath( o.RoutePrefix, route ),
                IsLongRunning = isLongRunning,
                HandlerType = typeof( THandler )
            } );

            return o;
        }
    }
}
