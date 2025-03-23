using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace prueba.Helpers
{
    public static class LoginHelper
    {
        public static string IsValidPassword(string password) =>
           password?.Length < 8 ? "La contraseña debe tener al menos 8 caracteres" :
           !password!.Any(char.IsUpper) ? "La contraseña debe tener al menos una letra mayúscula" :
           !password!.Any(char.IsLower) ? "La contraseña debe tener al menos una letra minúscula" :
           !password!.Any(char.IsDigit) ? "La contraseña debe tener al menos un número" :
           !password!.Any(c => !char.IsLetterOrDigit(c)) ? "La contraseña debe tener al menos un símbolo" :
           string.Empty;

        public static string IsValidEmail(string email) =>
            email?.Length < 8 ? "El correo debe tener al menos 8 caracteres" :
            !email!.Contains("@") ? "El correo debe tener al menos un @" :
            !email!.Contains(".") ? "El correo debe tener al menos un ." :
            string.Empty;

    }
}