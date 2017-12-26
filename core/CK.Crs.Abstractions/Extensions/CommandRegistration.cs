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
    class CommandRegistration : ICommandRegistration
    {
        readonly CommandModel _model;
        readonly ICommandRegistry _registry;

        public CommandModel Model => _model;

        /// <summary>
        /// Creates a <see cref="CommandRegistration"/> from a <see cref="CommandModel"/>
        /// </summary>
        /// <param name="model"></param>
        public CommandRegistration( ICommandRegistry registry, CommandModel model )
        {
            _registry = registry;
            _model = model;
        }

        CommandRegistration HandledBy<THandler>()
        {
            _model.HandlerType = typeof( THandler );

            return this;
        }

        CommandRegistration CommandName( string commandName )
        {
            _model.Name = commandName;
            return this;
        }

        CommandRegistration CustomBinder<T>()
        {
            _model.Binder = typeof( T );
            return this;
        }

        ICommandConfiguration<ICommandRegistration> ICommandConfiguration<ICommandRegistration>.CommandName( string commandName )
        {
            return CommandName( commandName );
        }

        ICommandConfiguration<ICommandRegistration> ICommandConfiguration<ICommandRegistration>.CustomBinder<T>()
        {
            return CustomBinder<T>();
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
