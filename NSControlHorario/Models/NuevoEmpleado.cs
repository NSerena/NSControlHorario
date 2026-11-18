
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NSControlHorarioApp.Models
{
    public class NuevoEmpleado
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string Nombre { get; set; } = "";

        [System.ComponentModel.DataAnnotations.Required]
        public string Apellidos { get; set; } = "";

        public string DNI { get; set; } = "";

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        public string Email { get; set; } = "";

        [System.ComponentModel.DataAnnotations.Required]
        public string Password { get; set; } = "";
    }
}
