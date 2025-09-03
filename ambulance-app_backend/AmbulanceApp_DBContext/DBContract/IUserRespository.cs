using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmbulanceApp.Models.UserModel;

namespace AmbulanceApp_DBContext.DBContract
{
    public interface IUserRespository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByPhoneAsync(string phone);
        Task<User?> GetOrCreateByPhoneAsync(string phone);
        Task<User?> GetOrCreateByEmailAsync(string email);
        Task SaveAsync(User user);
    }
}
