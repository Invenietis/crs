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
    public static class CommandRegistryExtensions
    {
        /// <summary>
        /// Simply iterates over all given assemblies name and register all command handler found.
        /// </summary>
        /// <remarks>
        /// Given assemblies must exists in the output folder of the solution, otherwise they will not be loaded.
        /// </remarks>
        /// <param name="registry"></param>
        /// <param name="monitor">An optional <see cref="IActivityMonitor"/></param>
        public static ICommandRegistry AutoRegisterSimpleCurrentAssembly(this ICommandRegistry registry, IActivityMonitor monitor = null)
            => registry.AutoRegisterSimple(new AutoRegisterOption(Assembly.GetEntryAssembly().GetName() ), monitor);

        /// <summary>
        /// Simply iterates over all given assemblies name and register all command handler found.
        /// </summary>
        /// <remarks>
        /// Given assemblies must exists in the output folder of the solution, otherwise they will not be loaded.
        /// </remarks>
        /// <param name="registry"></param>
        /// <param name="monitor">An optional <see cref="IActivityMonitor"/></param>
        /// <param name="assemblies">A list of assembly name.</param>
        public static ICommandRegistry AutoRegisterSimple(this ICommandRegistry registry, IActivityMonitor monitor = null, params string[] assemblies)
            => registry.AutoRegisterSimple(new AutoRegisterOption(assemblies), monitor);

        /// <summary>
        /// Uses the given <see cref="AutoRegisterOption"/> to auto-register handlers.
        /// </summary>
        /// <remarks>
        /// Given assemblies must exists in the output folder of the solution, otherwise they will not be loaded.
        /// </remarks>
        /// <param name="registry">The registry holding registered commands.</param>
        /// <param name="option">Auto registration option.</param>
        /// <param name="monitor">An optional <see cref="IActivityMonitor"/></param>
        public static ICommandRegistry AutoRegisterSimple(this ICommandRegistry registry, AutoRegisterOption option, IActivityMonitor monitor = null)
        {
            if (monitor == null) monitor = new ActivityMonitor();

            foreach (var a in option.Assemblies)
            {
                using (monitor.OpenTrace().Send("Discovering types in assembly {0}", a))
                {
                    try
                    {
                        monitor.Trace().Send("Loading assembly {0}", a);
                        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(a));
                        monitor.Trace().Send("Assembly {0} loaded successfuly", assembly.FullName);
                        var handlers = assembly.GetTypes().Where(t => typeof(ICommandHandler).IsAssignableFrom(t)).Where(option.CommandHandlerFilter).ToArray();
                        if (handlers.Length == 0)
                        {
                            monitor.Warn().Send("No handler found...");
                        }
                        else
                        {
                            foreach (var h in handlers)
                            {
                                using (monitor.OpenTrace().Send("Registering handler {0}", h.AssemblyQualifiedName))
                                {
                                    foreach (var commandType in h.GetInterfaces()
                                        .Where(interfaceType => interfaceType != typeof(ICommandHandler))
                                        .Where(interfaceType => typeof(ICommandHandler).IsAssignableFrom(interfaceType))
                                        .Where(interfaceType => interfaceType.GetTypeInfo().IsGenericType && interfaceType.GenericTypeArguments.Length == 1)
                                        .Select(interfaceType => interfaceType.GenericTypeArguments[0]))
                                    {
                                        CommandDescription description = new CommandDescription(commandType)
                                        {
                                            Name = option.CommandNameProvider(commandType),
                                            HandlerType = h
                                        };
                                        description.Description = option.CommandDescriptionProvider(description.CommandType);
                                        description.Traits = option.CommandTraitsProvider(description.CommandType);
                                        description.Decorators = option.CommandDecorators(description.CommandType, description.HandlerType);
                                        registry.Register(description);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        monitor.Error().Send(ex, "An error occured...");
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
        public static ICommandRegistration Register<TCommand>(this ICommandRegistry registry)
            where TCommand : class
        {
            var d = new CommandDescription(typeof(TCommand));
            var registration = new CommandRegistration(registry, d);
            registry.Register(d);
            return registration;
        }

    }
}