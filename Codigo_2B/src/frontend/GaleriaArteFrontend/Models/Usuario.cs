using System;
using System.ComponentModel.DataAnnotations;

namespace GaleriaArteFrontend.Models
{
    public class Usuario
    {
        public string Nickname { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "El usuario o correo es requerido")]
        public string Identificador { get; set; } = string.Empty; // nickname o correo

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Contraseña { get; set; } = string.Empty;
    }

    public class RegistroRequest
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [MinLength(3, ErrorMessage = "El nombre de usuario debe tener al menos 3 caracteres")]
        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede exceder 50 caracteres")]
        public string Nickname { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        public string Contraseña { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirma tu contraseña")]
        [Compare("Contraseña", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContraseña { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecciona un tipo de usuario")]
        public string Rol { get; set; } = "cliente"; // Rol por defecto
    }

    public class SolicitarRecuperacionRequest
    {
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string Correo { get; set; } = string.Empty;
    }

    public class RestablecerPasswordRequest
    {
        public Guid Token { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        public string NuevaPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirma tu nueva contraseña")]
        [Compare("NuevaPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarPassword { get; set; } = string.Empty;
    }

    public class ApiResponse
    {
        public string Mensaje { get; set; } = string.Empty;
        public bool Exito { get; set; }
    }
}
