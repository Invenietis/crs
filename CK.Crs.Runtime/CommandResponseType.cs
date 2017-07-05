using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Defines all possible response types of a command
    /// </summary>
    public enum CommandResponseType
    {
        /// <summary>
        /// This is a returned when validation failed on a command (Filtering step).
        /// </summary>
        ValidationError =   'V',
        /// <summary>
        /// This is a returnd when an error has been raised by the execution of the command, in the command handler. (Execution step).
        /// </summary>
        InternalError =     'I',
        /// <summary>
        /// This is returned when the command has successfuly been executed in a synchronous-way, and a result is directly accessible by the client.
        /// </summary>
        Synchronous =       'S',
        /// <summary>
        /// This is returned when the execution of the command has been deferred by the pipeline.
        /// </summary>
        Asynchronous =      'A',
        /// <summary>
        /// This is returned for any meta command result.
        /// </summary>
        Meta =              'M'
    }
}
