
namespace NSControlHorarioApp.Models
{
    public class InformeHoras
    {
        public string UserId { get; set; } = string.Empty;

        public string NombreEmpleado { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }

        // Horas realmente trabajadas.
        public decimal HorasTrabajadas { get; set; }

        // Horas previstas según el horario habitual.
        public decimal HorasPrevistas { get; set; }

        // Horas correspondientes a vacaciones.
        public decimal HorasVacaciones { get; set; }

        // Horas correspondientes a bajas.
        public decimal HorasBaja { get; set; }

        // Horas de permisos retribuidos.
        public decimal HorasPermisoRetribuido { get; set; }

        // Horas de permisos no retribuidos.
        public decimal HorasPermisoNoRetribuido { get; set; }

        // Indica si hay una entrada sin su correspondiente salida.
        public bool TieneFichajePendiente { get; set; }

        // Incidencias o aclaraciones del día.
        public string Observaciones { get; set; } = string.Empty;
    }
}
