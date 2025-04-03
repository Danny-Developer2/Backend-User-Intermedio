using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace prueba.Entities
{
    public class RegisterAsistencia
    {
        public Guid Id { get; set; } 

        public Guid UserId { get; set; } = Guid.Empty;

        public DateTime Fecha { get; set; } = DateTime.Now;

        public bool Asistencia { get; set; } = false;

        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        [JsonIgnore]
        public User User { get; set; } = null!;
    }
}