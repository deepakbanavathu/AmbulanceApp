using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbulanceApp.Models.UserModel
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? PasswordHash { get; set; } = null;
        public bool IsActive { get; set; } = true;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    }
}
