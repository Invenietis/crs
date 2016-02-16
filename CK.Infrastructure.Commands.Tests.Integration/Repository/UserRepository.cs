using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class UserRepository
    {
        static Dictionary<string, UserModel> store = new Dictionary<string, UserModel>();

        public static void Add( UserModel user )
        {
            user.Id = Guid.NewGuid().ToString();
            store[user.Id] = user;
        }

        public static IEnumerable<UserModel> Get()
        {
            return store.Select( kp => kp.Value ).ToArray();
        }

        public static UserModel Get( string id )
        {
            return store[id];
        }
    }
}
