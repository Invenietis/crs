using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class AccountRepository
    {
        
        static Dictionary<string, AccountModel> store = new Dictionary<string, AccountModel>();

        public static void Add( AccountModel account )
        {
            account.Id = Guid.NewGuid().ToString();
            store[account.Id] = account;
        }

        public static IEnumerable<AccountModel> Get()
        {
            return store.Select(kp => kp.Value).ToArray();
        }

        public static AccountModel Get(string id)
        {
            return store[id];
        }
    }
}
