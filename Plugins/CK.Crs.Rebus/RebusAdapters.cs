using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Extensions;
using Rebus.Handlers;
using Rebus.Pipeline;
using Rebus.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers and/or modifies Rebus configuration for the current service collection.
        /// </summary>
        /// <param name="services">The current message service builder.</param>
        /// <param name="configureRebus">The optional configuration actions for Rebus.</param>
        public static IServiceCollection AddRebus( this IServiceCollection services, Func<RebusConfigurer, RebusConfigurer> configureRebus )
        {
            return AddRebus( services, ( c, p ) => configureRebus( c ) );
        }

        /// <summary>
        /// Registers and/or modifies Rebus configuration for the current service collection.
        /// </summary>
        /// <param name="services">The current message service builder.</param>
        /// <param name="configureRebus">The optional configuration actions for Rebus.</param>
        public static IServiceCollection AddRebus( this IServiceCollection services, Func<RebusConfigurer, IServiceProvider, RebusConfigurer> configureRebus )
        {
            if( configureRebus == null )
            {
                throw new ArgumentNullException( nameof( configureRebus ) );
            }

            var messageBusRegistration = services.FirstOrDefault( descriptor => descriptor.ServiceType == typeof( IBus ) );

            if( messageBusRegistration != null )
            {
                throw new InvalidOperationException( "Rebus has already been configured." );
            }

            services.AddTransient( s => MessageContext.Current );
            services.AddTransient( s => s.GetService<IBus>().Advanced.SyncBus );

            // Register the Rebus Bus instance, to be created when it is first requested.
            services.AddSingleton( provider => new NetCoreServiceProviderContainerAdapter( provider ) );
            services.AddSingleton( provider =>
            {
                var configurer = Configure.With( provider.GetRequiredService<NetCoreServiceProviderContainerAdapter>() );
                return configureRebus( configurer, provider ).Start();
            } );

            return services;
        }
    }


    /// <summary>
    /// Implementation of <see cref="IContainerAdapter"/> that is backed by a ServiceProvider
    /// </summary>
    /// <seealso cref="Rebus.Activation.IContainerAdapter" />
    public class NetCoreServiceProviderContainerAdapter : IContainerAdapter
    {
        readonly IServiceProvider _provider;

        IBus _bus;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetCoreServiceProviderContainerAdapter"/> class.
        /// </summary>
        /// <param name="provider">The service provider used to yield handler instances.</param>
        public NetCoreServiceProviderContainerAdapter( IServiceProvider provider )
        {
            _provider = provider ?? throw new ArgumentNullException( nameof( provider ) );
        }

        /// <summary>
        /// Resolves all handlers for the given <typeparamref name="TMessage"/> message type
        /// </summary>
        /// <exception cref="System.InvalidOperationException"></exception>
        public Task<IEnumerable<IHandleMessages<TMessage>>> GetHandlers<TMessage>( TMessage message, ITransactionContext transactionContext )
        {
            var scope = _provider.CreateScope();

            var resolvedHandlerInstances = GetMessageHandlersForMessage<TMessage>( scope );

            transactionContext.OnDisposed( ctx => scope.Dispose() );

            return Task.FromResult( (IEnumerable<IHandleMessages<TMessage>>)resolvedHandlerInstances.ToArray() );
        }

        /// <summary>
        /// Sets the bus instance associated with this <see cref="T:Rebus.Activation.IContainerAdapter" />.
        /// </summary>
        /// <param name="bus"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetBus( IBus bus )
        {
            if( _bus != null )
            {
                throw new InvalidOperationException( "Cannot set the bus instance more than once on the container adapter." );
            }

            _bus = bus ?? throw new ArgumentNullException( nameof( bus ) );
        }

        private List<IHandleMessages<TMessage>> GetMessageHandlersForMessage<TMessage>( IServiceScope scope )
        {
            var handledMessageTypes = typeof( TMessage ).GetBaseTypes()
                .Concat( new[] { typeof( TMessage ) } );

            return handledMessageTypes
                .SelectMany( t =>
                {
                    var implementedInterface = typeof( IHandleMessages<> ).MakeGenericType( t );

                    return scope.ServiceProvider.GetServices( implementedInterface ).Cast<IHandleMessages>();
                } )
                .Cast<IHandleMessages<TMessage>>()
                .ToList();
        }

        #region IDisposable Support

        private bool disposedValue = false;

        /// <summary>
        /// Disposes of the bus.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose( bool disposing )
        {
            if( !disposedValue )
            {
                if( disposing )
                {
                    _bus?.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Disposes of the bus.
        /// </summary>
        public void Dispose()
        {
            Dispose( true );
        }

        #endregion
    }

    /// <summary>
    /// Extension methods for making it easy to register Rebus handlers in your <see cref="IServiceCollection"/>
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Automatically picks up all handler types from the assembly containing <typeparamref name="THandler"/> and registers them in the container
        /// </summary>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        /// <param name="services">The services.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void AutoRegisterHandlersFromAssemblyOf<THandler>( this IServiceCollection services )
        {
            if( services == null )
                throw new ArgumentNullException( nameof( services ) );

            var assemblyToRegister = GetAssembly<THandler>();

            RegisterAssembly( services, assemblyToRegister );
        }

        /// <summary>
        /// Automatically picks up all handler types from the specified assembly and registers them in the container
        /// </summary>
        /// <param name="services">The services</param>
        /// <param name="assemblyString">The long name of the assembly</param>
        public static void AutoRegisterHandlersFromAssembly( this IServiceCollection services, string assemblyString )
        {
            if( services == null )
                throw new ArgumentNullException( nameof( services ) );

            if( string.IsNullOrEmpty( assemblyString ) )
                throw new ArgumentNullException( nameof( assemblyString ) );

            var assemblyName = new AssemblyName( assemblyString );

            var assembly = Assembly.Load( assemblyName );

            RegisterAssembly( services, assembly );
        }

        static Assembly GetAssembly<THandler>()
        {
            return typeof( THandler ).Assembly;
        }

        static IEnumerable<Type> GetImplementedHandlerInterfaces( Type type )
        {
            return type.GetInterfaces()
                .Where( i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof( IHandleMessages<> ) );
        }

        static void RegisterAssembly( IServiceCollection services, Assembly assemblyToRegister )
        {
            var typesToAutoRegister = assemblyToRegister.GetTypes()
                .Where( IsClass )
                .Select( type => new
                {
                    Type = type,
                    ImplementedHandlerInterfaces = GetImplementedHandlerInterfaces( type ).ToList()
                } )
                .Where( a => a.ImplementedHandlerInterfaces.Any() );

            foreach( var type in typesToAutoRegister )
            {
                RegisterType( services, type.Type, true );
            }
        }

        static bool IsClass( Type type )
        {
            return !type.IsInterface && !type.IsAbstract;
        }

        static void RegisterType( IServiceCollection services, Type typeToRegister, bool auto )
        {
            var implementedHandlerInterfaces = GetImplementedHandlerInterfaces( typeToRegister ).ToArray();

            if( !implementedHandlerInterfaces.Any() ) return;

            Array.ForEach( implementedHandlerInterfaces, i => services.AddTransient( i, typeToRegister ) );
        }
    }
    /// <summary>
    /// Defines common operations for Rebus the use an <see cref="IServiceProvider"/>.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Activates the Rebus engine, allowing it to start sending and receiving messages.
        /// </summary>
        /// <param name="provider">The service provider configured for Rebus.</param>
        public static IServiceProvider UseRebus( this IServiceProvider provider )
        {
            provider.GetRequiredService<IBus>();
            return provider;
        }

        /// <summary>
        /// Activates the Rebus engine, allowing it to start sending and receiving messages.
        /// </summary>
        /// <param name="provider">The service provider configured for Rebus.</param>
        /// <param name="busAction">An action to perform on the bus.</param>
        public static IServiceProvider UseRebus( this IServiceProvider provider, Action<IBus> busAction )
        {
            busAction( provider.GetRequiredService<IBus>() );
            return provider;
        }
    }
}
