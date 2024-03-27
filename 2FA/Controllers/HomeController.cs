﻿using _2FA.Infraestructura;
using _2FA.Models;
using Microsoft.AspNetCore.Mvc;
using TwoFactorAuthNet;
using TwoFactorAuthNet.Providers.Qr;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _2FA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly ICuentasServices _cuentasServices;

        public HomeController(ICuentasServices cuentasServices)
        { 
            _cuentasServices = cuentasServices;
        }

        [HttpGet("registros", Name = "GetRegistros")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Registros()
        {
            var Lista = await _cuentasServices.TodosLosRegistros();
            return Ok(Lista);
        }

        [HttpPost("crear", Name = "PostCrear")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Crear(string nombre, string contrasena, string Correo)
        {
            var cuenta = new Cuentas { Nombre = nombre, Contrasena = contrasena };
            await _cuentasServices.Crear(cuenta);
            return Ok();
        }

        [HttpGet, Route("GetQRCode")]
        public async Task<IActionResult> GetQRCode(string correo)
        {
            var tfa = new TwoFactorAuth("DosFac", 6, 30, Algorithm.SHA256, new ImageChartsQrCodeProvider());
            var secret = tfa.CreateSecret(160);

            var cuentas = new Cuentas { Correo = correo, SecretCode = secret };
            await _cuentasServices.SetSecret(cuentas);

            string imgQR = tfa.GetQrCodeImageAsDataUri(correo, secret);
            string imgHTML = $"<img src='{imgQR}'>";
            return Content(imgHTML, "text/html");
        }


        [HttpGet, Route("GetQRCodeAsImage")]
        public async Task<IActionResult> GetQRCodeAsImage(string correo)
        {
            var tfa = new TwoFactorAuth("DosFac", 6, 30, Algorithm.SHA256, new ImageChartsQrCodeProvider());
            var secret = tfa.CreateSecret(160);

            var cuentas = new Cuentas { Correo = correo, SecretCode = secret };
            await _cuentasServices.SetSecret(cuentas);

            string imgQR = tfa.GetQrCodeImageAsDataUri(correo, secret);

            // Verificar si la cadena base64 comienza con el prefijo correcto
            if (imgQR.StartsWith("data:image/png;base64,"))
            {
                // Eliminar el prefijo para obtener solo la cadena base64
                imgQR = imgQR.Substring("data:image/png;base64,".Length);

                // Intentar convertir la cadena base64 a un arreglo de bytes
                try
                {
                    byte[] picture = Convert.FromBase64String(imgQR);
                    return File(picture, "image/png");
                }
                catch (FormatException ex)
                {
                    // Manejar el error de conversión
                    return StatusCode(500, "Error al convertir la cadena base64 a un arreglo de bytes: " + ex.Message);
                }
            }
            else
            {
                // La cadena base64 no está en el formato esperado
                return StatusCode(500, "La cadena base64 no está en el formato esperado.");
            }
        }

        [HttpGet, Route("ValidarQRCode")]
        public async Task<bool> ValidarCofdigo(string correo, string code)
        {

            var secret = await _cuentasServices.GetSecret(correo);

            var tfa = new TwoFactorAuth("DosFac", 6, 30, Algorithm.SHA256);
            return tfa.VerifyCode(secret, code);
        }

    }
}