using System.Threading.Tasks;

namespace CK.Crs
{
    public interface IClientStore
    {
        Task AddClient( string clientId );

        Task RemoveClient( string clientId );
    }
}
