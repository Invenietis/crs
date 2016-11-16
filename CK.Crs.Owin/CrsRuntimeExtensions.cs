using System;
using CK.Crs.Runtime;

namespace CK.Crs
{
    public static class CrsRuntimeExtensions
    {
        [Obsolete( "Uses UseDefaultWithSyncExector instead" )]
        static public IPipelineBuilder UseDefault( this IPipelineBuilder builder )
        {
            return builder.UseDefaultWithSyncExecutor();
        }

        static public IPipelineBuilder UseDefaultWithSyncExecutor( this IPipelineBuilder builder )
        {
            return builder.Clear()
                .UseMetaComponent()
                .UseCommandRouter()
                .UseJsonCommandBuilder()
                .UseAmbientValuesValidator()
                .UseFilters()
                .UseSyncCommandExecutor()
                .UseJsonCommandWriter();
        }
    }
}
