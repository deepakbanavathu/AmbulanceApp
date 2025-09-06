using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbulanceApp_DBContext.Entities
{
    public class UserEntity
    {        
        public Guid Id { get; set; }
        [MaxLength(100)]
        public string? Email { get; set; }
        [MaxLength(15)]
        public string? Phone { get; set; }
        [MaxLength(200)]
        public string? PasswordHash { get; set; }
        [MaxLength(100)]
        public string? FullName { get; set; }
        public Guid RoleId { get; set; }
        public DateTime CreatedDt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
