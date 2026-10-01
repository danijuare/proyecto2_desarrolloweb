using System;

namespace proyecto_2_desarrollo_web.DTOs
{
    // Token pyahu oñembosako'íva usuario-pe haguã
    public class TokenSesionCreateDto
    {
        public int id_usuario { get; set; }
        public string token { get; set; } = string.Empty;
        public DateTime fecha_expiracion { get; set; }
    }

    // Respuesta JSON cliente-pe haguã
    public class TokenSesionResponseDto
    {
        public int id_token { get; set; }
        public int id_usuario { get; set; }
        public string token { get; set; } = string.Empty;
        public DateTime fecha_emision { get; set; }
        public DateTime fecha_expiracion { get; set; }
        public bool revocado { get; set; }
    }

    // Revocar (Anular) token rehegua
    public class RevocarTokenDto
    {
        public string token { get; set; } = string.Empty;
    }
}