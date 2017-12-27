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
        /// Registers a command and its handler.
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static ICommandRegistration Register<TCommand>( this ICommandRegistry registry )
            where TCommand : class
        {
            var model = new CommandModel( typeof( TCommand ), registry.TraitContext );
            var registration = new CommandRegistration( registry, model );
            registry.Register( model );
            if( model.ResultType != null )
            {
                registration.IsResultTag();
            }
            return registration;
        }

        public static ICommandRegistry Register<TCommand, THandler>( this ICommandRegistry registry )
            where TCommand : class
        {
            registry.Register<TCommand>().HandledBy<THandler>();
            return registry;
        }

    }
}
