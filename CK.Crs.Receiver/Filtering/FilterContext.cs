﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Filtering
{
    public class FilterContext : ICommandFilterContext
    {
        public FilterContext( IActivityMonitor monitor, CommandRoute description, ClaimsPrincipal principal, object command )
        {
            Monitor = monitor;
            Description = description;
            Command = command;
            User = principal;
        }

        public IActivityMonitor Monitor { get; }

        public CommandRoute Description { get; }

        public object Command { get; }

        /// <summary>
        /// Rejects the command before its execution.
        /// </summary>
        /// <param name="reason"></param>
        public void Reject( string reason )
        {
            Rejected = true;
            RejectReason = reason;
        }

        public bool Rejected { get; private set; }

        public string RejectReason { get; private set; }

        public ClaimsPrincipal User { get; }
    }
}
