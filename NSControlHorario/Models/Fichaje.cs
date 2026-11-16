
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSControlHorarioApp.Models
{
    public enum TipoFichaje
    {
        Entrada = 1,
        Salida = 2
    }

    public class Fichaje
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public DateTime FechaHoraUtc { get; set; }

        public TipoFichaje Tipo { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? Usuario { get; set; }
    }
}
