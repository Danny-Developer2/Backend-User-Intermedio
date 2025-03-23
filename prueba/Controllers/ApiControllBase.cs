using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using prueba.Error;

namespace prueba.Controllers
{
    public abstract class ApiControllBase : ControllerBase
    {
        //TODO metodo para manejar errores 
        protected ActionResult<ApiResponse> HandleNotFound(string message = "Resource not found")
        => NotFound(new ApiResponse(mensaje: message, exito: false));

        protected ActionResult<ApiResponse> HandleBadRequest(string message)
        => BadRequest(new ApiResponse(mensaje: message, exito: false));

        protected ActionResult<ApiResponse> HandleSuccess(string message, object? data = null)
        => Ok(new ApiResponse(mensaje: message, exito: true, datos: data));

    }
}