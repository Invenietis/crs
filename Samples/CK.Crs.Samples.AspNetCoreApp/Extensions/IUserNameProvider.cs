using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Core
{

    public interface IUserNameProvider
    {
        /// <summary>
        /// Gets the user name of the authenticated user.
        /// </summary>
        /// <returns>The user name.</returns>
        Task<string> GetAuthenticatedUserNameAsync();
    }
}
