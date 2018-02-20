using System;
using System.Collections.Generic;
using CK.Core;

namespace CK.Crs
{
    class DefaultCommandRegistry : ICommandRegistry
    {
        List<ICommandModel> _commands = new List<ICommandModel>();

        public DefaultCommandRegistry( CKTraitContext traitContext )
        {
            TraitContext = traitContext;
        }

        public IEnumerable<ICommandModel> Registration => _commands;

        public void Register( ICommandModel descriptor ) => _commands.Add( descriptor );

        public CKTraitContext TraitContext { get; }

        /// <summary>
        /// Registers a command and its handler.
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="registry"></param>
        /// <returns></returns>
        public ICommandRegistration Register<TCommand>() where TCommand : class
        {
            var model = new CommandModel( typeof( TCommand ), TraitContext );
            return AddRegistration( model );
        }

        public ICommandRegistration Register<TCommand, TResult>() where TCommand : ICommand<TResult>
        {
            var model = new CommandModel( typeof( TCommand ), typeof( TResult ), TraitContext );
            return AddRegistration( model );
        }

        private ICommandRegistration AddRegistration( CommandModel model )
        {
            var registration = new CommandRegistration( this, model );
            Register( model );
            if( model.ResultType != null )
            {
                registration.SetResultTag();
            }
            return registration;
        }

        public ICommandModel GetCommandByName( CommandName name )
        {
            return _commands.Find( n => n.Name == name );
        }
    }
}
