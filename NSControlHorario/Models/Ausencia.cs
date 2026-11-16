
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSControlHorarioApp.Models
{
    public enum TipoAusencia
    {
        Vacaciones = 1,
        BajaMedica = 2,
        PermisoRetribuido = 3,
        PermisoNoRetribuido = 4,
        Otros = 5
    }

    public class Ausencia
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public TipoAusencia Tipo { get; set; }

        // Fecha y hora de inicio de la ausencia (UTC).
        public DateTime FechaInicioUtc { get; set; }

        // Fecha y hora de finalización de la ausencia (UTC).
        public DateTime FechaFinUtc { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; } = string.Empty;

        [Required]
        public string UsuarioRegistroId { get; set; } = string.Empty;

        public DateTime FechaRegistroUtc { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? Usuario { get; set; }

        [ForeignKey(nameof(UsuarioRegistroId))]
        public ApplicationUser? UsuarioRegistro { get; set; }
    }
}
