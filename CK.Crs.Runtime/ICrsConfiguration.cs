using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{
    public interface ICrsConfiguration
    {
        /// <summary>
        /// Gets the receiver path.
        /// </summary>
        string ReceiverPath { get; }
        
        /// <summary>
        /// Gets the <see cref="ICommandRouteCollection"/> 
        /// </summary>
        IReadOnlyDictionary<CommandRoutePath, CommandRoute> Routes { get; }

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
        CKTraitContext TraitContext { get; set; }
    }

}
