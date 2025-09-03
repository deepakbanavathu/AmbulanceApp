using AmbulanceApp.Models.UserModel;
using AmbulanceApp_DBContext.DBContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbulanceApp_DBContext.Repository
{
    public class UserRepository : IUserRespository
    {
        public Task<User?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByPhoneAsync(string phone)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetOrCreateByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetOrCreateByPhoneAsync(string phone)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
