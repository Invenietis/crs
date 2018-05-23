using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CK.Crs.Owin
{
    class FilterProvider : ICommandFilterProvider
    {
        private readonly IServiceProvider _applicationServices;

        public FilterProvider( IServiceProvider applicationServices )
        {
            _applicationServices = applicationServices;
        }

        public IEnumerable<ICommandFilter> GetFilters( ICommandContext context, IEndpointModel model )
        {
            if( context == null )
            {
                throw new ArgumentNullException( nameof( context ) );
            }

            if( model == null )
            {
                throw new ArgumentNullException( nameof( model ) );
            }

            var requiredFilters = MergeFilters( context.Model, model );
            return OrderFilters( CreateFilters( requiredFilters, context ) );
        }

        protected virtual IEnumerable<ICommandFilter> CreateFilters( IEnumerable<Type> requiredFilters, ICommandContext context )
        {
            foreach( var filterType in requiredFilters )
            {
                if( !typeof( ICommandFilter ).IsAssignableFrom( filterType ) ) throw new InvalidOperationException( "Invalid command filter detected" );

                var httpContextFeature = context.GetFeature<IOwinContextCommandFeature>();
                var filter = (ICommandFilter)ActivatorUtilities.GetServiceOrCreateInstance( _applicationServices, filterType );
                yield return filter;
            }
        }

        protected virtual IEnumerable<ICommandFilter> OrderFilters( IEnumerable<ICommandFilter> filters )
        {
            return filters.OrderBy( x => x is ICommandSecurityFilter );
        }

        private IEnumerable<Type> MergeFilters( IFilterable model1, IFilterable model2 )
        {
            if( model1.Filters == null && model2.Filters == null ) return Type.EmptyTypes;
            if( model1.Filters == null ) return model2.Filters;
            if( model2.Filters == null ) return model1.Filters;

            return model1.Filters.Concat( model2.Filters ).Distinct( EqualityComparer<Type>.Default );
        }
    }
}
