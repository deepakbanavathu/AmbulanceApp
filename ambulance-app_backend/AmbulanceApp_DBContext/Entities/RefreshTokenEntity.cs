using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AmbulanceApp_DBContext.Entities
{
    public class RefreshTokenEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = default!;
        [MaxLength(256)]
        public string TokenHash { get; set; } = string.Empty!;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }
        public bool IsRevoked { get; set; } = false;
        //public bool IsActive => RevokedAt == null && !IsExpired;
        //public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    }
}
