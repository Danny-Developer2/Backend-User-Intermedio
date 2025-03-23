using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace prueba.Dto
{
    public class UserDTO
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime? LastLogin { get; set; } = DateTime.Now;


        public string Password { get; set; } = string.Empty;
       
        public string Phone { get; set; } = string.Empty;

        
        // public string Token { get; set; } = string.Empty;
    }
}