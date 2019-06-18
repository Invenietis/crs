using System.Threading.Tasks;

namespace CK.Crs
{
    public interface ICommandFilter
    {
        Task OnFilterAsync( CommandFilterContext filterContext );
    }
}
