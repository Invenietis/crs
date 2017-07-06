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
    public class CommandRegistration : ICommandRegistration
    {
        CommandDescription _commandDescription;
        ICommandRegistry _registry;
        CKTraitContext _traits;

        public CommandDescription Description  => _commandDescription;

        /// <summary>
        /// Creates a <see cref="CommandRegistration"/> from a <see cref="CommandDescription"/>
        /// </summary>
        /// <param name="description"></param>
        public CommandRegistration(ICommandRegistry registry, CommandDescription description, CKTraitContext traits)
        {
            _registry = registry;
            _commandDescription = description;
            _traits = traits;
        }

        public CKTraitContext Traits => _traits;

        CommandRegistration HandledBy<THandler>()
        {
            _commandDescription.HandlerType = typeof( THandler );

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
            var t = _traits.FindOrCreate("Async");
            if (_commandDescription.Traits == null) _commandDescription.Traits = t;
            else _commandDescription.Traits = _commandDescription.Traits.Union(t);

            return this;
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
    }

}
