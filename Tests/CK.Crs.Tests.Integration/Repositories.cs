using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Tests.Integration
{
    public interface IRepository<T>
    {
        IQueryable<T> Query { get;}

        void Add( T model );

        void Remove( T model );
    }

    public class UserModel
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class UserRepository : IRepository<UserModel>
    {
        Dictionary<Guid, UserModel> _users = new Dictionary<Guid, UserModel>();

        public IQueryable<UserModel> Query
        {
            get { return _users.Values.AsQueryable(); }
        }

        public void Add( UserModel model )
        {
            _users.Add( model.Id, model );
        }

        public void Remove( UserModel model )
        {
            _users.Remove( model.Id  );
        }
    }
}
