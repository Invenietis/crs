using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace CK.Crs.Configuration
{

    class CrsCoreBuilder : ICrsCoreBuilder
    {
        private readonly IServiceCollection _services;
        private readonly ICommandRegistry _registry;
        private readonly ICrsModel _model;
        private readonly IList<Type> _receivers;
        private readonly Dictionary<string, Type> _dispatchers;
        private readonly List<Func<IServiceProvider, ICommandReceiver>> _receiverFuncs;

        public CrsCoreBuilder( CrsConfigurationBuilder builder )
        {
            _model = builder.BuildModel();
            if( _model == null ) throw new ArgumentException( "CrsConfigurationBuilder must returns a valid ICrsModel." );

            _services = builder.Services;
            _registry = builder.Registry;
            _receivers = new List<Type>();
            _receiverFuncs = new List<Func<IServiceProvider, ICommandReceiver>>();
            _dispatchers = new Dictionary<string, Type>();
            _services.AddSingleton( _model );
            _services.AddScoped<ICommandReceiver, CompositeCommandReceiver>( s =>
            {
                return new CompositeCommandReceiver(
                    Receivers
                        .Select( r => s.GetRequiredService( r ) )
                        .Union( _receiverFuncs.Select( f => f( s ) ) )
                        .Union( new[] { ActivatorUtilities.CreateInstance<DefaultCommandReceiver>( s ) } )
                        .OfType<ICommandReceiver>() );
            } );
            _services.AddOptions();
            _services.Configure<ResultDispatcherOptions>( o =>
            {
                o.Dispatchers = _dispatchers;
            } );
        }

        public IList<Type> Receivers => _receivers;

        public IServiceCollection Services => _services;

        public ICommandRegistry Registry => _registry;

        public ICrsModel Model => _model;

        public void AddReceiver<T>() where T : class, ICommandReceiver
        {
            if( typeof( T ).IsAbstract ) throw new ArgumentException( "You should provide a concrete implementation type of ICommandReceiver" );
            _receivers.Add( typeof( T ) );
            // Don't do this: The scoped CompositeCommandReceiver registered in the constructor
            // handles dispatch of Commands
            //_services.AddSingleton<ICommandReceiver, T>( ( sp ) => sp.GetService<T>() );
        }

        public void AddReceiver( Func<IServiceProvider, ICommandReceiver> createReceiver )
        {
            _receiverFuncs.Add( createReceiver );
        }

        public void AddDispatcher<T>( string protocol ) where T : class, IResultDispatcher
        {
            if( String.IsNullOrEmpty( protocol ) ) throw new ArgumentNullException( nameof( protocol ) );
            _dispatchers.Add( protocol, typeof( T ) );
        }
    }
}
