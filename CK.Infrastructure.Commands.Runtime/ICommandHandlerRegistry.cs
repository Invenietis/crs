//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CK.Infrastructure.Commands
//{
//    /// <summary>
//    /// Defines the contract for a registry of <see cref="ICommandHandler"/>
//    /// </summary>
//    public interface ICommandHandlerRegistry
//    {
//        /// <summary>
//        /// Registers an handler to the command handler registry
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <typeparam name="THandler">Must implement <see cref="ICommandHandler"/></typeparam>
//        void RegisterHandler<T, THandler>() 
//            where T : class 
//            where THandler : ICommandHandler<T>;

//        /// <summary>
//        /// Retuns the type of a registered handler for the given <paramref name="commandType"/> or null.
//        /// </summary>
//        /// <param name="commandType">The <see cref="Type"/> of a command</param>
//        /// <returns>The <see cref="Type"/> of the handler</returns>
//        Type GetHandlerType( Type commandType );
//    }

//}
