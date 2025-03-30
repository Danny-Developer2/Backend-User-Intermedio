using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace prueba.Entities
{
    public class Empleados
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;

        public int Edad { get; set; } 

        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Puesto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}