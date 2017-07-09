﻿using CK.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Describes a command and its environment.
    /// </summary>
    public class RequestDescription
    {
        internal static string RemoveSuffixes( Type t, params string[] suffixes)
        {
            var s = t.Name;
            foreach (var suf in suffixes)
            {
                if (s.EndsWith(suf, StringComparison.OrdinalIgnoreCase))
                {
                    int idx = s.IndexOf(suf);
                    return s.Substring(0, idx);
                }
            }
            return s;
        }

        /// <summary>
        /// Creates a command description
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="handlerType"></param>
        public RequestDescription( Type type )
        {
            Type = type ?? throw new ArgumentNullException( nameof( type ) );
            Name = type.Name;
            Decorators = CK.Core.Util.Array.Empty<Type>();
            Traits = null;
        }

        /// <summary>
        /// The name of the command
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The <see cref="System.Type"/> of the command to process.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The <see cref="System.Type"/> of the command to process.
        /// </summary>
        public Type HandlerType { get; set; }

        /// <summary>
        /// Gets commands traits
        /// This gives a hint to CRS to pick the best command executor.
        /// </summary>
        public CKTrait Traits { get; set; }

        /// <summary>
        /// Gets or sets a description for this command. Can you any format like Markdown or HTML.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a read-only collection of decorators that should be applied when this command is handled.
        /// A decorator encapsulate common behaviors or cross-cutting-concerns accross commands of a system, like authorization, logging, transaction etc.
        /// </summary>
        public IReadOnlyCollection<Type> Decorators { get; set; }

        IDictionary<string, object> _extraData;
        /// <summary>
        /// Gets a dictionary of extra data.
        /// </summary>
        public IDictionary<string, object> ExtraData
        {
            get { return _extraData ?? (_extraData = new Dictionary<string, object>()); }
        }
    }
}