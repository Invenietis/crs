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
    public static class CommandRegistryExtensions
    {
        /// <summary>
        /// Simply iterates over all given assemblies name and register all command handler found.
        /// </summary>
        /// <remarks>
        /// Given assemblies must exists in the output folder of the solution, otherwise they will not be loaded.
        /// </remarks>
        /// <param name="r"></param>
        /// <param name="monitor">An optional <see cref="IActivityMonitor"/></param>
        /// <param name="assemblies">A list of assembly name.</param>
        public static void AutoRegisterSimple( this ICommandRegistry registry, IActivityMonitor monitor = null, params string[] assemblies )
            => registry.AutoRegisterSimple( new AutoRegisterOption( assemblies ), monitor );

        /// <summary>
        /// Uses the given <see cref="AutoRegisterOption"/> to auto-register handlers.
        /// </summary>
        /// <remarks>
        /// Given assemblies must exists in the output folder of the solution, otherwise they will not be loaded.
        /// </remarks>
        /// <param name="registry">The registry holding registered commands.</param>
        /// <param name="option">Auto registration option.</param>
        /// <param name="monitor">An optional <see cref="IActivityMonitor"/></param>
        public static void AutoRegisterSimple( this ICommandRegistry registry, AutoRegisterOption option, IActivityMonitor monitor = null )
        {
            if( monitor == null ) monitor = new ActivityMonitor();

            foreach( var a in option.Assemblies )
            {
                using( monitor.OpenTrace().Send( "Discovering types in assembly {0}", a ) )
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom( a );
                        var handlers = assembly.GetTypes().Where( t => typeof( ICommandHandler<>  ).IsAssignableFrom( t) ).Where( option.CommandHandlerFilter );

                        foreach( var h in handlers )
                        {
                            using( monitor.OpenTrace().Send( "Registering handler {0}", h.AssemblyQualifiedName ) )
                            {
                                CommandDescription description = new CommandDescription();
                                description.HandlerType = h;
                                monitor.Trace().Send( "...HandlerType is {0}", description.HandlerType.Name );

                                description.CommandType = h.GenericTypeArguments[0];
                                monitor.Trace().Send( "...CommandType is {0}", description.CommandType.Name );

                                description.Name = option.CommandNameProvider( description.CommandType );
                                monitor.Trace().Send( "...CommandName is {0}", description.CommandType.Name );

                                description.Description = option.CommandDescriptionProvider( description.CommandType );
                                description.Traits = option.CommandTraitsProvider( description.CommandType );
                                description.Decorators = option.CommandDecorators( description.CommandType, description.HandlerType );
                                registry.Register( description );
                            }
                        }
                    }
                    catch( Exception ex )
                    {
                        monitor.Error().Send( ex, "An error occured..." );
                    }
                }
            }
        }

        /// <summary>
        /// Registers a command and its handler.
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
            var d = new CommandDescription
            {
                Name = GetDefaultName( typeof(TCommand)),
                CommandType = typeof( TCommand),
                HandlerType = typeof(THandler)
            };
            d.Decorators = ExtractDecoratorsFromHandlerAttributes( d.CommandType, d.HandlerType ).ToArray();

            var registration = new CommandRegistration( d );
            registry.Register( d );
            return registration;
        }

        public static IReadOnlyCollection<Type> ExtractDecoratorsFromHandlerAttributes( Type commandType, Type handlerType ) =>
            handlerType.GetTypeInfo().GetCustomAttributes().OfType<ICommandDecorator>().Select( t => t.GetType() ).ToArray();

        public static string GetDefaultName( Type type ) => type.Name.RemoveSuffixes( "Command", "Cmd" );

        static string RemoveSuffixes( this string s, params string[] suffixes )
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