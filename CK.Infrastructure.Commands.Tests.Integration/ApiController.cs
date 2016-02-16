using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CK.Infrastructure.Commands.WebIntegration
{
    [Route("api")]
    public class ApiController : Controller
    {
        [HttpGet("account")]
        public IEnumerable<AccountModel> GetAccounts()
        {
            return AccountRepository.Get();
        }

        [HttpGet( "account/{id}" )]
        public AccountModel Get(string id)
        {
            return AccountRepository.Get(id);
        }

        [HttpGet( "user" )]
        public IEnumerable<UserModel> GetUsers()
        {
            return UserRepository.Get();
        }

        [HttpGet( "user/{id}" )]
        public UserModel GetUser( string id )
        {
            return UserRepository.Get( id );
        }
    }
}