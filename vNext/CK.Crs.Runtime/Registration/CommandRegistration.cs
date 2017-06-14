using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    /// <summary>
    /// Implementation of the fluent registration API.
    /// </summary>
    public class CommandRegistration : ICommandRegistrationWithFilter, ICommandRegistration
    {
        CommandRoute _CommandRoute;
        CommandDescription _commandDescription;
        ICommandRegistry _registry;
        /// <summary>
        /// Creates a <see cref="CommandRegistration"/> from a <see cref="CommandRoute"/>
        /// </summary>
        /// <param name="route"></param>
        public CommandRegistration( ICommandRegistry registry, CommandRoute route )
        {
            _registry = registry;
            _CommandRoute = route;
            _commandDescription = route.Descriptor;
        }

        /// <summary>
        /// Creates a <see cref="CommandRegistration"/> from a <see cref="CommandDescription"/>
        /// </summary>
        /// <param name="description"></param>
        public CommandRegistration(ICommandRegistry registry, CommandDescription description )
        {
            _registry = registry;
            _commandDescription = description;
        }

        CommandRegistration HandledBy<THandler>() where THandler : ICommandHandler
        {
            _commandDescription.HandlerType = typeof( THandler );
            _commandDescription.Decorators = CommandDescription.ExtractDecoratorsFromHandlerAttributes(
                _commandDescription.CommandType, 
                _commandDescription.HandlerType).ToArray();

            return this;
        }

        CommandRegistration AddDecorator<TDecorator>() where TDecorator : ICommandDecorator
        {
            _commandDescription.Decorators = _commandDescription.Decorators.Union( new[] { typeof( TDecorator ) } ).ToArray();
            return this;
        }

        CommandRegistration AddExtraData( string key, object value )
        {
            _commandDescription.ExtraData.Add( key, value );
            return this;
        }

        CommandRegistration CommandName( string commandName )
        {
            _commandDescription.Name = commandName;
            return this;
        }

        CommandRegistration IsAsync()
        {
            _commandDescription.Traits = "Async";
            return this;
        }

        CommandRegistration AddFilter<TFilter>() where TFilter : ICommandFilter
        {
            if( _CommandRoute == null ) throw new NotSupportedException( "Cannot add filters in this registration context" );
            _CommandRoute.Filters = _CommandRoute.Filters.Union( new[] { typeof( TFilter ) } ).ToArray();
            return this;
        }

        ICommandConfiguration<ICommandRegistrationWithFilter> ICommandConfiguration<ICommandRegistrationWithFilter>.AddDecorator<T1>()
        {
            return AddDecorator<T1>();
        }

        ICommandConfiguration<ICommandRegistrationWithFilter> ICommandConfiguration<ICommandRegistrationWithFilter>.CommandName( string commandName )
        {
            return CommandName( commandName );
        }

        ICommandConfiguration<ICommandRegistrationWithFilter> ICommandConfiguration<ICommandRegistrationWithFilter>.IsAsync()
        {
            return IsAsync();
        }

        ICommandConfiguration<ICommandRegistrationWithFilter> ICommandConfiguration<ICommandRegistrationWithFilter>.AddExtraData( string key, object value )
        {
            return AddExtraData( key, value );
        }

        ICommandConfigurationWithFilter<ICommandRegistrationWithFilter> ICommandConfigurationWithFilter<ICommandRegistrationWithFilter>.AddFilter<T1>()
        {
            return AddFilter<T1>();
        }

        ICommandConfiguration<ICommandRegistration> ICommandConfiguration<ICommandRegistration>.AddDecorator<T1>()
        {
            return AddDecorator<T1>();
        }

        ICommandConfiguration<ICommandRegistration> ICommandConfiguration<ICommandRegistration>.AddExtraData( string key, object value )
        {
            return AddExtraData( key, value );
        }

        ICommandConfiguration<ICommandRegistration> ICommandConfiguration<ICommandRegistration>.CommandName( string commandName )
        {
            return CommandName( commandName );
        }

        ICommandConfiguration<ICommandRegistration> ICommandConfiguration<ICommandRegistration>.IsAsync()
        {
            return IsAsync();
        }


        ICommandConfiguration<ICommandRegistration> ICommandConfigurationWithHandling<ICommandRegistration>.HandledBy<THandler>()
        {
            return HandledBy<THandler>();
        }

        ICommandRegistration ICommandConfiguration<ICommandRegistration>.Register<TCommand>()
        {
            return _registry.Register<TCommand>();
        }

        ICommandRegistration ICommandConfiguration<ICommandRegistrationWithFilter>.Register<TCommand>()
        {
            return _registry.Register<TCommand>();
        }
    }

}
