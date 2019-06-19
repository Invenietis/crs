using System.Collections.Generic;

namespace CK.Crs
{
    public interface ICommandFilterProvider
    {
        /// <summary>
        /// Gets all filters to applies
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        IEnumerable<ICommandFilter> GetFilters( ICommandContext context, IEndpointModel model );
    }
}
