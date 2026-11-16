
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSControlHorarioApp.Models
{
    public class ModificacionFichaje
    {
        public int Id { get; set; }

        public int FichajeId { get; set; }

        [Required]
        public string UsuarioModificacionId { get; set; } = string.Empty;

        [Required]
        public DateTime FechaModificacionUtc { get; set; }

        public DateTime FechaHoraAnteriorUtc { get; set; }

        public TipoFichaje TipoAnterior { get; set; }

        public DateTime FechaHoraNuevaUtc { get; set; }

        public TipoFichaje TipoNuevo { get; set; }

        [Required]
        [StringLength(500)]
        public string Motivo { get; set; } = string.Empty;

        [ForeignKey(nameof(FichajeId))]
        public Fichaje? Fichaje { get; set; }

        [ForeignKey(nameof(UsuarioModificacionId))]
        public ApplicationUser? UsuarioModificacion { get; set; }
    }
}
