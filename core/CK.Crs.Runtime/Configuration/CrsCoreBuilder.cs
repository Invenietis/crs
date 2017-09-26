using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace CK.Crs
{
    public class ResultDispatcherOptions
    {
        public Dictionary<string, Type> Dispatchers { get; set; }

    }

    public class CrsCoreBuilder : ICrsCoreBuilder
    {
        private readonly IServiceCollection _services;
        private readonly ICommandRegistry _registry;
        private readonly ICrsModel _model;
        private readonly IList<Type> _receivers;

        private readonly Dictionary<string, Type> _dispatchers;

        public CrsCoreBuilder( CrsConfigurationBuilder builder )
        {
            _model = builder.BuildModel();
            if( _model == null ) throw new ArgumentException( "CrsConfigurationBuilder must returns a valid ICrsModel." );

            _services = builder.Services;
            _registry = builder.Registry;
            _receivers = new List<Type>();
            _dispatchers = new Dictionary<string, Type>();
            _services.AddSingleton( _model );
            _services.AddSingleton<ICommandReceiver, CompositeCommandReceiver>( s =>
            {
                return new CompositeCommandReceiver(
                    Receivers
                        .Select( r => s.GetRequiredService( r ) )
                        .Union( new[] { ActivatorUtilities.CreateInstance<DefaultCommandReceiver>( s ) } )
                        .OfType<ICommandReceiver>() );
            } );
            _services.Configure<ResultDispatcherOptions>( o =>
            {
                o.Dispatchers = _dispatchers;
            } );
        }

        public IList<Type> Receivers => _receivers;

        public IServiceCollection Services => _services;

        public ICommandRegistry Registry => _registry;

        public ICrsModel Model => _model;

        public void AddReceiver<T>( Func<IServiceProvider, T> factoryFunction = null ) where T : class, ICommandReceiver
        {
            if( typeof( T ).IsAbstract ) throw new ArgumentException( "You should provided a concrete command receiver" );
            _receivers.Add( typeof( T ) );

            if( factoryFunction != null ) _services.AddSingleton( factoryFunction );
            else _services.AddSingleton<T>();
        }

        public void AddDispatcher<T>( string protocol ) where T : class, IResultDispatcher
        {
            if( String.IsNullOrEmpty( protocol ) ) throw new ArgumentNullException( nameof( protocol ) );
            _dispatchers.Add( protocol, typeof( T ) );
        }
    }
}
