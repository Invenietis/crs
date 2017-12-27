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
        public static ICommandConfiguration<ICommandRegistration> Register<TCommand, THandler>( this ICommandRegistry registry )
            where TCommand : class
        {
            return registry.Register<TCommand>().HandledBy<THandler>();
        }

    }
}
