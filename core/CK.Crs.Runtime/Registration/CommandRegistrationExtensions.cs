using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
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
        ///// <summary>
        ///// Simply iterates over all given assemblies name and register all command handler found.
        ///// </summary>
        ///// <remarks>
        ///// Given assemblies must exists in the output folder of the solution, otherwise they will not be loaded.
        ///// </remarks>
        ///// <param name="registry"></param>
        ///// <param name="monitor">An optional <see cref="IActivityMonitor"/></param>
        //public static ICommandRegistry AutoRegisterSimpleCurrentAssembly(this ICommandRegistry registry, IActivityMonitor monitor = null)
        //    => registry.AutoRegisterSimple(new AutoRegisterOption(Assembly.GetEntryAssembly().GetName()), monitor);

        /// <summary>
        /// Simply iterates over all given assemblies name and register all command handler found.
        /// </summary>
        /// <remarks>
        /// Given assemblies must exists in the output folder of the solution, otherwise they will not be loaded.
        /// </remarks>
        /// <param name="registry"></param>
        /// <param name="monitor">An optional <see cref="IActivityMonitor"/></param>
        /// <param name="assemblies">A list of assembly name.</param>
        public static IRequestRegistry RegisterAssemblies( this IRequestRegistry registry, params string[] assemblies )
            => registry.AutoRegisterSimple( new AutoRegisterOption( assemblies ) );

        /// <summary>
        /// Uses the given <see cref="AutoRegisterOption"/> to auto-register handlers.
        /// </summary>
        /// <remarks>
        /// Given assemblies must exists in the output folder of the solution, otherwise they will not be loaded.
        /// </remarks>
        /// <param name="registry">The registry holding registered commands.</param>
        /// <param name="option">Auto registration option.</param>
        /// <param name="monitor">An optional <see cref="IActivityMonitor"/></param>
        public static IRequestRegistry AutoRegisterSimple( this IRequestRegistry registry, AutoRegisterOption option, IActivityMonitor monitor = null )
        {
            if( monitor == null ) monitor = new ActivityMonitor();

            foreach( var a in option.Assemblies )
            {
                using( monitor.OpenTrace( $"Discovering types in assembly {a}" ) )
                {
                    try
                    {
                        monitor.Trace( $"Loading assembly {0}" );
                        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName( new AssemblyName( a ) );
                        monitor.Trace( $"Assembly {assembly.FullName} loaded successfuly" );

                        var handlers = assembly.GetTypes()
                            .Where( t => typeof( IRequestHandler ).IsAssignableFrom( t ) )
                            .ToArray();
                        if( handlers.Length == 0 )
                        {
                            monitor.Warn( "No handler found..." );
                        }
                        else
                        {
                            foreach( var h in handlers )
                            {
                                using( monitor.OpenTrace( $"Registering handler {h.AssemblyQualifiedName}" ) )
                                {
                                    foreach( var commandType in h.GetInterfaces()
                                        .Where( t => typeof( IRequestHandler ).IsAssignableFrom( t ) )
                                        .Where( interfaceType => interfaceType.GetTypeInfo().IsGenericType && interfaceType.GenericTypeArguments.Length > 0 )
                                        .Select( interfaceType => interfaceType.GenericTypeArguments[0] ) )
                                    {
                                        RequestDescription description = new RequestDescription( commandType )
                                        {
                                            Name = option.CommandNameProvider( commandType ),
                                            HandlerType = h
                                        };
                                        description.Description = option.CommandDescriptionProvider( description.Type );
                                        description.Traits = option.CommandTraitsProvider( description.Type );
                                        registry.Register( description );
                                    }
                                }
                            }
                        }
                    }
                    catch( Exception ex )
                    {
                        monitor.Error( "An error occured...", ex );
                    }
                }
            }

            return registry;
        }

        /// <summary>
        /// Registers a command and its handler.
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static ICommandRegistration Register<TCommand>( this IRequestRegistry registry )
            where TCommand : class
        {
            var d = new RequestDescription( typeof( TCommand ) );
            var registration = new CommandRegistration( registry, d, registry.TraitContext );
            registry.Register( d );
            return registration;
        }

    }
}