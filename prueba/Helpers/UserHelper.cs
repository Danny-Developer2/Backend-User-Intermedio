using System.Linq;
using System.Text.RegularExpressions;

namespace prueba.Helpers
{
    public static class UserHelper
    {
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled);

        public static string ValidateFirstName(string firstName) =>
            string.IsNullOrEmpty(firstName) ? "El nombre es requerido" :
            firstName.Length < 3 ? "El nombre debe tener al menos 3 caracteres" :
            firstName.Length > 50 ? "El nombre debe tener menos de 50 caracteres" :
            !firstName.All(char.IsLetter) ? "El nombre solo debe contener letras" :
            string.Empty;

        public static string ValidateLastName(string lastName) =>
            string.IsNullOrEmpty(lastName) ? "El apellido es requerido" :
            lastName.Length < 3 ? "El apellido debe tener al menos 3 caracteres" :
            lastName.Length > 50 ? "El apellido debe tener menos de 50 caracteres" :
            !lastName.All(char.IsLetter) ? "El apellido solo debe contener letras" :
            string.Empty;

        public static string ValidateEmail(string email) =>
            string.IsNullOrEmpty(email) ? "El email es requerido" :
            !EmailRegex.IsMatch(email) ? "El formato del email no es válido" :
            string.Empty;

        public static string ValidatePhone(string phone) =>
            string.IsNullOrEmpty(phone) ? "El teléfono es requerido" :
            phone.Count(char.IsDigit) != 10 ? "El teléfono debe tener exactamente 10 dígitos" :
            !PhoneRegex.IsMatch(phone) ? "El formato del teléfono no es válido" :
            string.Empty;

        public static string ValidateToken(string token) =>
            string.IsNullOrEmpty(token) ? "El token es requerido" : string.Empty;

        public static string ValidateLastLogin(DateTime? lastLogin) =>
            !lastLogin.HasValue ? "La fecha de último login es requerida" :
            lastLogin > DateTime.Now ? "La fecha de último login no puede ser futura" :
            string.Empty;
    }
}