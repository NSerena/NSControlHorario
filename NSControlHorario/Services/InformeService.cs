
using Microsoft.EntityFrameworkCore;
using NSControlHorario.Data;
using NSControlHorarioApp.Models;

namespace NSControlHorarioApp.Services
{
    public class InformeService
    {
        private readonly ApplicationDbContext _context;

        public InformeService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<(decimal HorasTrabajadas, bool TieneFichajePendiente)> CalcularHorasDiaAsync(string userId, DateTime fecha)
        {
            var zonaMadrid = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");

            var inicioLocal = fecha.Date;
            var finLocal = inicioLocal.AddDays(1);

            var inicioUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(inicioLocal, DateTimeKind.Unspecified), zonaMadrid);

            var finUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(finLocal, DateTimeKind.Unspecified), zonaMadrid);

            var fichajes = await _context.Fichajes
                .Where(f => f.UserId == userId &&
                            f.FechaHoraUtc >= inicioUtc &&
                            f.FechaHoraUtc < finUtc)
                .OrderBy(f => f.FechaHoraUtc)
                .ToListAsync();

            decimal minutosTrabajados = 0;
            DateTime? entradaPendiente = null;

            foreach (var fichaje in fichajes)
            {
                if (fichaje.Tipo == TipoFichaje.Entrada)
                {
                    entradaPendiente = fichaje.FechaHoraUtc;
                }
                else if (fichaje.Tipo == TipoFichaje.Salida && entradaPendiente.HasValue)
                {
                    minutosTrabajados += (decimal)(
                        fichaje.FechaHoraUtc - entradaPendiente.Value
                    ).TotalMinutes;

                    entradaPendiente = null;
                }
            }

            return (
                Math.Round(minutosTrabajados / 60m, 2),
                entradaPendiente.HasValue
            );
        }


        public async Task<Dictionary<TipoAusencia, decimal>> CalcularAusenciasDiaAsync(string userId, DateTime fecha)
        {
            var zonaMadrid = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");

            var inicioLocal = fecha.Date;
            var finLocal = inicioLocal.AddDays(1);

            var inicioUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(inicioLocal, DateTimeKind.Unspecified), zonaMadrid);
            var finUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(finLocal, DateTimeKind.Unspecified), zonaMadrid);

            var ausencias = await _context.Ausencias
                .Where(a => a.UserId == userId &&
                            a.FechaInicioUtc < finUtc &&
                            a.FechaFinUtc > inicioUtc)
                .ToListAsync();

            var resultado = new Dictionary<TipoAusencia, decimal>();

            foreach (var ausencia in ausencias)
            {
                var inicio = ausencia.FechaInicioUtc > inicioUtc
                    ? ausencia.FechaInicioUtc
                    : inicioUtc;

                var fin = ausencia.FechaFinUtc < finUtc
                    ? ausencia.FechaFinUtc
                    : finUtc;

                var horas = (decimal)(fin - inicio).TotalHours;

                if (resultado.ContainsKey(ausencia.Tipo))
                    resultado[ausencia.Tipo] += horas;
                else
                    resultado[ausencia.Tipo] = horas;
            }

            return resultado.ToDictionary(
                x => x.Key,
                x => Math.Round(x.Value, 2));
        }


        public async Task<InformeHoras> GenerarInformeDiaAsync(string userId, string nombreEmpleado, DateTime fecha)
        {
            var (horasTrabajadas, fichajePendiente) =
                await CalcularHorasDiaAsync(userId, fecha);

            var ausencias = await CalcularAusenciasDiaAsync(userId, fecha);
            var ausenciasDetalle = await ObtenerAusenciasDiaAsync(userId, fecha);

            var incidencias = new List<string>();

            if (fichajePendiente)
                incidencias.Add("Fichaje pendiente: falta registrar una salida.");

            foreach (var ausencia in ausenciasDetalle)
            {
                if (ausencia.Tipo == TipoAusencia.Vacaciones)
                {
                    incidencias.Add("Vacaciones (día completo)");
                }
                else
                {
                    var inicioLocal = TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.SpecifyKind(ausencia.FechaInicioUtc, DateTimeKind.Utc),
                        TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"));

                    var finLocal = TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.SpecifyKind(ausencia.FechaFinUtc, DateTimeKind.Utc),
                        TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"));

                    incidencias.Add(
                        $"{ausencia.Tipo} ({inicioLocal:HH:mm} - {finLocal:HH:mm})");
                }
            }

            if (fichajePendiente)
                incidencias.Add("Fichaje pendiente: falta registrar una salida.");

            if (ausencias.GetValueOrDefault(TipoAusencia.Vacaciones) > 0)
                incidencias.Add("Vacaciones");

            if (ausencias.GetValueOrDefault(TipoAusencia.BajaMedica) > 0)
                incidencias.Add("Baja médica");

            if (ausencias.GetValueOrDefault(TipoAusencia.PermisoRetribuido) > 0)
                incidencias.Add("Permiso retribuido");

            if (ausencias.GetValueOrDefault(TipoAusencia.PermisoNoRetribuido) > 0)
                incidencias.Add("Permiso no retribuido");

            if (ausencias.GetValueOrDefault(TipoAusencia.Otros) > 0)
                incidencias.Add("Otros");

            return new InformeHoras
            {
                UserId = userId,
                NombreEmpleado = nombreEmpleado,
                Fecha = fecha.Date,
                HorasTrabajadas = horasTrabajadas,
                HorasVacaciones = ausencias.GetValueOrDefault(TipoAusencia.Vacaciones),
                HorasBaja = ausencias.GetValueOrDefault(TipoAusencia.BajaMedica),
                HorasPermisoRetribuido = ausencias.GetValueOrDefault(TipoAusencia.PermisoRetribuido),
                HorasPermisoNoRetribuido = ausencias.GetValueOrDefault(TipoAusencia.PermisoNoRetribuido),
                TieneFichajePendiente = fichajePendiente,
                Observaciones = string.Join(" | ", incidencias)
            };
        }

        public async Task<List<InformeHoras>> GenerarInformePeriodoAsync(string userId, string nombreEmpleado, DateTime fechaDesde, DateTime fechaHasta)
        {
            var informes = new List<InformeHoras>();

            for (var fecha = fechaDesde.Date; fecha <= fechaHasta.Date; fecha = fecha.AddDays(1))
            {
                var informe = await GenerarInformeDiaAsync(
                    userId, nombreEmpleado, fecha);

                informes.Add(informe);
            }

            return informes;
        }

        public async Task<List<ApplicationUser>> ObtenerEmpleadosAsync()
        {
            return await _context.Users
                .Where(u => u.Activo)
                .OrderBy(u => u.Apellidos)
                .ThenBy(u => u.Nombre)
                .ToListAsync();
        }

        public async Task<List<InformeHoras>> GenerarInformeTodosEmpleadosAsync(DateTime fechaDesde, DateTime fechaHasta)
        {
            var empleados = await ObtenerEmpleadosAsync();
            var informes = new List<InformeHoras>();

            foreach (var empleado in empleados)
            {
                var nombreCompleto = $"{empleado.Nombre} {empleado.Apellidos}".Trim();

                var informesEmpleado = await GenerarInformePeriodoAsync(
                    empleado.Id,
                    nombreCompleto,
                    fechaDesde,
                    fechaHasta);

                informes.AddRange(informesEmpleado);
            }

            return informes;
        }

        public async Task<List<InformeHoras>> GenerarInformeEmpleadoAsync(string userId, DateTime fechaDesde, DateTime fechaHasta)
        {
            var empleado = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (empleado == null)
                return new List<InformeHoras>();

            var nombreCompleto = $"{empleado.Nombre} {empleado.Apellidos}".Trim();

            return await GenerarInformePeriodoAsync(
                empleado.Id,
                nombreCompleto,
                fechaDesde,
                fechaHasta);
        }

        public async Task<List<Ausencia>> ObtenerAusenciasDiaAsync(string userId, DateTime fecha)
        {
            var zonaMadrid = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");

            var inicioLocal = fecha.Date;
            var finLocal = inicioLocal.AddDays(1);

            var inicioUtc = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(inicioLocal, DateTimeKind.Unspecified), zonaMadrid);

            var finUtc = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(finLocal, DateTimeKind.Unspecified), zonaMadrid);

            return await _context.Ausencias
                .Where(a => a.UserId == userId &&
                            a.FechaInicioUtc < finUtc &&
                            a.FechaFinUtc > inicioUtc)
                .OrderBy(a => a.FechaInicioUtc)
                .ToListAsync();
        }

    }
}
