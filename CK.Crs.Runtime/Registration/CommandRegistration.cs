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

        /// <summary>
        /// Creates a <see cref="CommandRegistration"/> from a <see cref="CommandRoute"/>
        /// </summary>
        /// <param name="route"></param>
        public CommandRegistration( CommandRoute route )
        {
            _CommandRoute = route;
            _commandDescription = route.Descriptor;
        }

        /// <summary>
        /// Creates a <see cref="CommandRegistration"/> from a <see cref="CommandDescription"/>
        /// </summary>
        /// <param name="description"></param>
        public CommandRegistration( CommandDescription description )
        {
            _commandDescription = description;
        }

        CommandRegistration HandledBy<THandler>() where THandler : ICommandHandler
        {
            _commandDescription = new CommandDescription( _commandDescription.Name, _commandDescription.CommandType, typeof( THandler ) );
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
            _commandDescription = new CommandDescription( commandName, _commandDescription.CommandType, _commandDescription.HandlerType );
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


        ICommandConfigurationWithHandling<ICommandRegistration> ICommandConfigurationWithHandling<ICommandRegistration>.HandledBy<THandler>()
        {
            return HandledBy<THandler>();
        }
    }

}
