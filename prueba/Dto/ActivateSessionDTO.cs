using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace prueba.Dto
{
    public class ActivateSessionDTO
    {
        public Guid UserId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime Expiration { get; set; } = DateTime.Now;
    }
}