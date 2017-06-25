﻿using CK.Core;
using CK.Crs.Runtime;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs.Scalability
{

    public interface ICrsStartup
    {
        void ConfigureServices(IServiceCollection services);

        void ConfigurePipeline( IPipelineBuilder pipeline );

        void Configure( ICrsBuilder app);
    } 
}
