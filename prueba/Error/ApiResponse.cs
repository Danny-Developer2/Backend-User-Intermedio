using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace prueba.Error
{
    public class ApiResponse
    {
        public string Mensaje { get; set; }
        public bool Exito { get; set; }
        public object? Datos { get; set; }
        public string? Error { get; set; }


        public ApiResponse(string mensaje, bool exito = true, object? datos = null, string? error = null)
        {
            Mensaje = mensaje;
            Exito = exito;
            Datos = datos;
            Error = error;
        }

    }
}