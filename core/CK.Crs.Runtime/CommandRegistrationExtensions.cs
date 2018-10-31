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
        public static ICommandRegistration Register<TCommand>( this ICommandRegistry registry ) where TCommand : class
        {
            var model = new CommandModel( typeof( TCommand ), registry.TraitContext );
            return AddRegistration( registry, model );
        }


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
            return registry.RegisterWithResult<TCommand, TResult>().HandledBy<THandler>();
        }

        public static ICommandRegistration RegisterWithResult<TCommand, TResult>( this ICommandRegistry registry ) where TCommand : ICommand<TResult>
        {
            var model = new CommandModel( typeof( TCommand ), typeof( TResult ), registry.TraitContext );
            return AddRegistration( registry, model );
        }

        static ICommandRegistration AddRegistration( ICommandRegistry registry, CommandModel model )
        {
            var registration = new CommandRegistration( registry, model );
            registry.Register( model );
            if( model.ResultType != null )
            {
                registration.SetResultTag();
            }
            return registration;
        }
    }
}
