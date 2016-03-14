using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CK.Crs.Tests.Integration
{
    [Route("api")]
    public class ApiController : Controller
    {
        readonly IRepository<UserModel> _repository;
        public ApiController( IRepository<UserModel> repository )
        {
            _repository = repository;
        }

        // GET: api/values
        [HttpGet("user")]
        public IEnumerable<UserModel> Get()
        {
            return _repository.Query.ToArray();
        }
    }
}
