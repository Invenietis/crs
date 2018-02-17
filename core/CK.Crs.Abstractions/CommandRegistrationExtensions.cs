using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    /// <summary>
    /// Registry extensions
    /// </summary>
    public static class CommandRegistrationExtensions
    {
        /// <summary>
        /// Registers a command with the given handler
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static ICommandRegistration Register<TCommand, THandler>( this ICommandRegistry registry )
            where TCommand : class
            where THandler : ICommandHandler<TCommand>
        {
            return registry.Register<TCommand>().HandledBy<THandler>();
        }
        /// <summary>
        /// Registers a command with the given handler
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static ICommandRegistration Register<TCommand, TResult, THandler>( this ICommandRegistry registry )
            where TCommand : ICommand<TResult>
            where THandler : ICommandHandler<TCommand, TResult>
        {
            return registry.Register<TCommand, TResult>().HandledBy<THandler>();
        }

    }
}
