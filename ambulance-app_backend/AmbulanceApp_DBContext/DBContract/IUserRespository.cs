using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmbulanceApp.Models.UserModel;
using AmbulanceApp_DBContext.Entities;

namespace AmbulanceApp_DBContext.DBContract
{
    public interface IUserRespository
    {
        Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<UserEntity?> GetByEmailAsync(string email, CancellationToken ct=default);
        Task<UserEntity?> GetByPhoneAsync(string phone, CancellationToken ct=default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
        Task<bool> PhoneExistsAsync(string phone, CancellationToken ct = default);
        Task CreateAsync(UserEntity user,CancellationToken ct = default);
        Task UpdateAsync(UserEntity user,CancellationToken ct = default);
        Task RemoveAsync(UserEntity user,CancellationToken ct = default);
        Task<UserEntity?> GetOrCreateByPhoneAsync(string phone, CancellationToken ct = default); // Func<UserEntity> factory
        Task<UserEntity?> GetOrCreateByEmailAsync(string email,  CancellationToken ct = default); //Func<UserEntity> factory,
        Task SaveAsync(CancellationToken ct = default);
    }
}
