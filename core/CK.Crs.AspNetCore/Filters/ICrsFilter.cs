using System.Threading.Tasks;

namespace CK.Crs.AspNetCore
{
    public interface ICrsFilter
    {
        Task ApplyFilterAsync( CrsFilterContext filterContext );
    }
}
