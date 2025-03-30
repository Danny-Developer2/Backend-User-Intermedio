using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace prueba.Dto
{
    public class RegisterAsistenciaDTO
    {
        public Guid UserId { get; set; } = Guid.Empty;

        public DateTime Fecha { get; set; } = DateTime.Now;

        public bool Asistencia { get; set; } = false;
    }
}