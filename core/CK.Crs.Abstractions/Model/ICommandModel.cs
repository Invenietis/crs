using System;
using CK.Core;

namespace CK.Crs
{
    /// <summary>
    /// Describes a command registered in the application.
    /// It holds everything about the metadata of a command.
    /// </summary>
    public interface ICommandModel : IFilterable, IBindable
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        CommandName Name { get; }
        /// <summary>
        /// A <see cref="CKTrait"/> exposings the command traits which are used by the command pipeline 
        /// to take decisions about the command handling.
        /// </summary>
        CKTrait Tags { get; set; }
        /// <summary>
        /// The .NET <see cref="Type"/> of the command.
        /// </summary>
        Type CommandType { get; }
        /// <summary>
        /// An optionnal description of the command.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// The .NET <see cref="Type"/> of the handler.
        /// </summary>
        Type HandlerType { get; }
        /// <summary>
        /// The .NET <see cref="Type"/> of the command result class. Can be null.
        /// </summary>
        Type ResultType { get; }
    }
}
