using System;
using System.Collections.Generic;
using CK.Core;

namespace CK.Crs
{
    class DefaultCommandRegistry : ICommandRegistry
    {
        Dictionary<CommandName, ICommandModel> Map { get; } = new Dictionary<CommandName, ICommandModel>();

        public DefaultCommandRegistry( CKTraitContext traitContext )
        {
            TraitContext = traitContext;
        }

        public IEnumerable<ICommandModel> Registration
        {
            get { return Map.Values; }
        }

        public CKTraitContext TraitContext { get; }

        public void Register( ICommandModel descriptor )
        {
            Map.Add( descriptor.Name, descriptor );
        }

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
            return Map.GetValueWithDefault( name, null );
        }
    }
}
