using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{
    /// <summary>
    /// Holds configuration specific to a CRS Handler.
    /// </summary>
    public interface ICrsConfiguration
    {
        /// <summary>
        /// Gets the receiver path.
        /// </summary>
        string ReceiverPath { get; }

        /// <summary>
        /// Gets the <see cref="ICommandRouteCollection"/> 
        /// </summary>
        ICommandRouteCollection Routes { get; }

        /// <summary>
        /// Gets the <see cref="PipelineEvents"/> 
        /// </summary>
        PipelineEvents Events { get; }

        /// <summary>
        /// Gets the <see cref="IPipelineBuilder"/>
        /// </summary>
        IPipelineBuilder Pipeline { get; }

        /// <summary>
        /// Gets or sets the <see cref="CKTraitContext"/>.
        /// </summary>
        CKTraitContext TraitContext { get; }

        /// <summary>
        /// Selects the commands from the <see cref="ICommandRegistry"/> this CommandReceiver is able to handle. 
        /// </summary>
        /// <param name="selection">A projection lambda to filter commands</param>
        /// <param name="globalConfiguration">Global configuration delegate</param>
        /// <returns><see cref="ICrsConfiguration"/></returns>
        ICrsConfiguration AddCommands( Func<ICommandRegistry, IEnumerable<CommandDescription>> selection, Action<ICommandRegistrationWithFilter> globalConfiguration = null );
    }

}
