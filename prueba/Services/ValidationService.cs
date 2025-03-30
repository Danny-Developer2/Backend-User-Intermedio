using prueba.Dto;
using prueba.Error;
using prueba.Helpers;

namespace prueba.Services
{
    public class ValidationService
    {
        public static ApiResponse ValidateUserRegistration(UserDTO userDto)
        {
            var validations = new List<string>
            {
                UserHelper.ValidateFirstName(userDto.FirstName),
                UserHelper.ValidateLastName(userDto.LastName),
                UserHelper.ValidateEmail(userDto.Email),
                UserHelper.ValidatePhone(userDto.Phone),
                LoginHelper.IsValidPassword(userDto.Password)
            };

            var error = validations.FirstOrDefault(v => !string.IsNullOrEmpty(v));
            if (!string.IsNullOrEmpty(error))
            {
                return new ApiResponse(
                    mensaje: error,
                    exito: false,
                    error: "Validation error"
                );
            }

            return null!;
        }
    }
}