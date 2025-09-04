using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AmbulanceApp_DBContext.DBContext;
using AmbulanceApp_DBContext.DBContract;
using AmbulanceApp_DBContext.Entities;
using Microsoft.EntityFrameworkCore;


namespace AmbulanceApp_DBContext.Repository
{
    public class UserRepository : IUserRespository
    {
        private readonly AmbulanceAppDBContext _db;
        public UserRepository(AmbulanceAppDBContext db) => _db = db;

        public Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

        public Task<UserEntity?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            var norm = NormalizeEmail(email);
            return _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == norm, ct);
        }

        public Task<UserEntity?> GetByPhoneAsync(string phone, CancellationToken ct = default)
        {
            var norm = NormalizePhone(phone);
            return _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Phone == norm, ct);
        }

        public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
            => _db.Users.AnyAsync(u => u.Email == NormalizeEmail(email), ct);

        public Task<bool> PhoneExistsAsync(string phone, CancellationToken ct = default)
            => _db.Users.AnyAsync(u => u.Phone == NormalizePhone(phone), ct);

        public Task CreateAsync(UserEntity user, CancellationToken ct = default)
        {
            user.Id = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id;
            user.Email = user.Email is null ? null : NormalizeEmail(user.Email);
            user.Phone = user.Phone is null ? null : NormalizePhone(user.Phone);
            user.CreatedAt = user.CreatedAt == default ? DateTime.UtcNow : user.CreatedAt;

            _db.Users.Add(user);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(UserEntity user, CancellationToken ct = default)
        {
            user.Email = user.Email is null ? null : NormalizeEmail(user.Email);
            user.Phone = user.Phone is null ? null : NormalizePhone(user.Phone);
            user.UpdateAt = DateTime.UtcNow;

            _db.Users.Update(user);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(UserEntity user, CancellationToken ct = default)
        {
            _db.Users.Remove(user);
            return Task.CompletedTask;
        }

        public async Task<UserEntity> GetOrCreateByPhoneAsync(string phone, CancellationToken ct = default) //Func<UserEntity> factory,
        {
            var norm = NormalizePhone(phone);
            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Phone == norm, ct);
            if (existing != null) return existing;

            var entity = new UserEntity
            {
                Phone = norm,
                Id = Guid.NewGuid(),
                RoleId = Guid.Empty,
                CreatedAt = DateTime.UtcNow
            };
            
            _db.Users.Add(entity);
            return entity;
        }

        public async Task<UserEntity> GetOrCreateByEmailAsync(string email,CancellationToken ct = default) // Func<UserEntity> factory, 
        {
            var norm = NormalizeEmail(email);
            var exsisting = await _db.Users.FirstOrDefaultAsync(u => u.Email == norm, ct);
            if (exsisting != null) return exsisting;

            var entity = new UserEntity
            {
                Email = norm,
                Id = Guid.NewGuid(),
                RoleId = Guid.Empty,
                CreatedAt = DateTime.UtcNow
            };          

            _db.Users.Add(entity);
            return entity;
        }

        public Task SaveAsync(CancellationToken ct = default) 
            => _db.SaveChangesAsync();

        private static string NormalizeEmail(string email)
            => email?.Trim().ToLowerInvariant() ?? string.Empty;

        private static string NormalizePhone(string phone)
        {
            if(string.IsNullOrWhiteSpace(phone)) return string.Empty;

            return phone.Trim().Replace(" ", " ").Replace("-", "");
        }

    }
}
