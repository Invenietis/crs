using CK.Core;
using CK.Crs.Runtime;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CK.Crs.Scalability.FileSystem
{
    public static class Extensions
    {
        public static ICrsBuilder UseFileSystem(this ICrsBuilder builder, string path)
        {
            builder.UseInputFileSystem(Path.Combine(path, "Inputs"));
            builder.UseOutputFileSystem(Path.Combine(path, "Outputs"));
            return builder;
        }

        public static ICrsBuilder UseInputFileSystem(this ICrsBuilder builder, string path)
        {
            builder.Input.AddListener(new FileSystemCommandReceiver(new FileSystemConfiguration(path), builder.Services.GetRequiredService<IJsonConverter>()));
            return builder;
        }

        public static ICrsBuilder UseOutputFileSystem(this ICrsBuilder builder, string path)
        {
            builder.Output.AddDispatcher(new FileSystemCommandDispatcher(new FileSystemConfiguration(path)));
            return builder;
        }

        public static IPipelineBuilder UseFileSystemCommandBus( this IPipelineBuilder builder, CKTraitContext traitContext,  FileSystemConfiguration configuration )
        {
            traitContext.FindOrCreate(Traits.Scalable);
            return builder.Use<FileSystemCommandBus>(configuration);
        }
    }

}
