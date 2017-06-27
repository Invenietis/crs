using System;
using System.Collections.Generic;
using System.Linq;
using CK.Core;
using CK.Crs.Runtime;

namespace CK.Crs.Scalability.Internals
{
    /// <summary>
    /// Main Crs configuration entry
    /// </summary>
    class CrsConfiguration : ICrsConfiguration
    {
        internal ICommandRegistry _registry;
        readonly HashSet<Type> _filters;
        string _path;
        internal CrsConfiguration( string path, ICommandRegistry registry )
        {
            _path = path;
            _registry = registry;

            _filters = new HashSet<Type>();

            Pipeline = new PipelineBuilder();
            Events = new PipelineEvents();
            TraitContext = new CKTraitContext( "CrsDefault" );
            ExternalComponents = new ExternalComponents();
        }

        /// <summary>
        /// Gets or sets the <see cref="CKTraitContext"/>
        /// </summary>
        public CKTraitContext TraitContext { get; set; }

        /// <summary>
        /// Gets the base path of this receiver
        /// </summary>
        public string ReceiverPath => _path;

        /// <summary>
        /// Gets the pipeline events configuration object.
        /// </summary>
        public PipelineEvents Events { get; }

        /// <summary>
        /// Gets access to the pipeline
        /// </summary>
        public IPipelineBuilder Pipeline { get; }

        /// <summary>
        /// External components configuration
        /// </summary>
        public ExternalComponents ExternalComponents { get; }
    }
}