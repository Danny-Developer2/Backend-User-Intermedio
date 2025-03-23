using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace prueba.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;  // Added this property

        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];

        public DateTime? LastLogin { get; set; } = DateTime.Now;


        public string Token { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new List<string> { "USER" };


    }

}