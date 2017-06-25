using CK.Crs.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs.Scalability
{
    /// <summary>
    ///Defines a <see cref="CommandRequest"/> factory.
    /// </summary>
    public interface ICommandRequestFactory
    {
        /// <summary>
        /// Creates a <see cref="CommandRequest"/> from a <see cref="CommandJob"/>
        /// </summary>
        CommandRequest CreateFrom(CommandJob message);
    }

}
