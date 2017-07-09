﻿using CK.Core;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using System;

namespace CK.Crs.Samples.AspNetCoreApp
{
    [Route("my-crs-public/[Action]")]
    public class CrsPublicEndpoint<T> : DefaultCrsEndpoint<T> where T : class
    {
        public CrsPublicEndpoint(IBus dispatcher) : base(dispatcher) { }

        [HttpPost, NoAmbientValuesValidation]
        public override Task<Response> ReceiveCommand([FromBody] T command, IActivityMonitor monitor, string callbackId)
             => base.ReceiveCommand(command, monitor, callbackId);
    }
}