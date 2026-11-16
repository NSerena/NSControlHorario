
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NSControlHorarioApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Apellidos { get; set; } = string.Empty;

        [StringLength(20)]
        public string? DNI { get; set; }

        public DateTime FechaAlta { get; set; } = DateTime.UtcNow;

        public bool Activo { get; set; } = true;
    }
}
