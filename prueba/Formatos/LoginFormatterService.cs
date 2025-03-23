using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using prueba.Dto;
using prueba.Helpers;

namespace prueba.Formatos
{
    public interface ILoginFormatterService
    {
        object LoginFormat(LoginDTO loginDTO);
    }
    public class LoginFormatterService: ILoginFormatterService
    {
        public object LoginFormat(LoginDTO loginDTO)
        {
           return new 
           {
              username = LoginHelper.IsValidEmail(loginDTO.Username),
              password = LoginHelper.IsValidPassword(loginDTO.Password)
           };
        }
        
    }
}