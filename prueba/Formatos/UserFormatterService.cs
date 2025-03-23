using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using prueba.Dto;
using prueba.Helpers;

namespace prueba.Formatos
{
    public interface IUserFormatterService
    {
     object FormatUser(UserDTO user);   
    }
    public class UserFormatterService : IUserFormatterService
    {
        public object FormatUser(UserDTO user)
        {
            return new
            {
                nombre = UserHelper.ValidateFirstName(user.FirstName),
                apellido = UserHelper.ValidateLastName(user.LastName),
                correo = UserHelper.ValidateEmail(user.Email),
                telefono = UserHelper.ValidatePhone(user.Phone),
                ultimo_login = UserHelper.ValidateLastLogin(user.LastLogin)
            };
           

        }
    }
}