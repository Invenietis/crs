using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CK.Crs
{
    public static class CommandRegistryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="route"></param>
        /// <param name="isLongRunning"></param>
        /// <returns></returns>
        public static ICommandRegistration Register<TCommand, THandler>( this ICommandRegistry registry )
            where TCommand : class
            where THandler : class, ICommandHandler<TCommand>
        {
            var defaultCommandDescriptor = new CommandDescription
            {
                Name = GetDefaultName( typeof(TCommand)),
                CommandType = typeof( TCommand),
                HandlerType = typeof(THandler),
                IsLongRunning = false
            };
            defaultCommandDescriptor.Decorators = ExtractDecoratorsFromHandlerAttributes(
                defaultCommandDescriptor.CommandType,
                defaultCommandDescriptor.HandlerType )
                .ToArray();

            var registration = new CommandRegistration( defaultCommandDescriptor );
            registry.Register( defaultCommandDescriptor );
            return registration;
        }

        private static IReadOnlyCollection<Type> ExtractDecoratorsFromHandlerAttributes( Type commandType, Type handlerType )
        {
            return handlerType.GetTypeInfo().CustomAttributes
                .Where( c => typeof( ICommandDecorator).IsAssignableFrom( c.AttributeType ) )
                .Select( a => a.AttributeType ).ToArray();
        }

        private static string GetDefaultName( Type type )
        {
            var n = type.Name;
            return n.RemoveSuffixes( "Command", "Cmd" );
        }
        
        public static string RemoveSuffixes( this string s, params string[] suffixes )
        {
            foreach( var suf in suffixes )
            {
                if( s.EndsWith( suf, StringComparison.OrdinalIgnoreCase ) )
                {
                    int idx = s.IndexOf(suf);
                    return s.Substring( 0, idx );
                }
            }
            return s;
        }
    }
}