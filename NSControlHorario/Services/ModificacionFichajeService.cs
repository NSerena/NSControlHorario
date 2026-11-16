using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSControlHorario.Data;
using NSControlHorarioApp.Models;

namespace NSControlHorarioApp.Services
{
    public class ModificacionFichajeService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ModificacionFichajeService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<(bool Correcto, string Mensaje)> ModificarAsync(
            ClaimsPrincipal usuarioActual,
            int fichajeId,
            DateTime nuevaFechaHoraUtc,
            TipoFichaje nuevoTipo,
            string motivo)
        {
            if (usuarioActual.Identity?.IsAuthenticated != true)
                return (false, "Debes iniciar sesión.");

            var usuarioId = usuarioActual.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioId))
                return (false, "No se ha podido identificar al usuario.");

            if (string.IsNullOrWhiteSpace(motivo))
                return (false, "Debes indicar el motivo.");

            if (motivo.Trim().Length > 500)
                return (false, "El motivo no puede superar los 500 caracteres.");

            var usuario = await _userManager.FindByIdAsync(usuarioId);

            if (usuario == null || !usuario.Activo)
                return (false, "Usuario no válido o inactivo.");

            var esAdministrador = await _userManager.IsInRoleAsync(
                usuario, "Administrador");

            var fichaje = await _context.Fichajes
                .FirstOrDefaultAsync(f => f.Id == fichajeId);

            if (fichaje == null)
                return (false, "No se ha encontrado el fichaje.");

            // Un empleado solo puede modificar sus propios fichajes.
            if (!esAdministrador && fichaje.UserId != usuarioId)
                return (false, "No tienes permiso para modificar este fichaje.");

            // Comprobar que la secuencia de fichajes sigue siendo correcta.
            var fichajesEmpleado = await _context.Fichajes
                .AsNoTracking()
                .Where(f => f.UserId == fichaje.UserId)
                .Select(f => new
                {
                    f.Id,
                    f.FechaHoraUtc,
                    f.Tipo
                })
                .ToListAsync();

            var secuencia = fichajesEmpleado
                .OrderBy(f => f.Id == fichajeId
                    ? nuevaFechaHoraUtc
                    : f.FechaHoraUtc)
                .ThenBy(f => f.Id)
                .ToList();

            var tipoEsperado = TipoFichaje.Entrada;

            foreach (var registro in secuencia)
            {
                var tipoComprobado = registro.Id == fichajeId
                    ? nuevoTipo
                    : registro.Tipo;

                if (tipoComprobado != tipoEsperado)
                {
                    return (false,
                        "No se puede realizar la modificación porque dejaría los fichajes en un orden incorrecto. " +
                        "Debe existir una entrada antes de cada salida.");
                }

                tipoEsperado = tipoEsperado == TipoFichaje.Entrada
                    ? TipoFichaje.Salida
                    : TipoFichaje.Entrada;
            }

            var modificacion = new ModificacionFichaje
            {
                FichajeId = fichaje.Id,
                UsuarioModificacionId = usuarioId,
                FechaModificacionUtc = DateTime.UtcNow,
                FechaHoraAnteriorUtc = fichaje.FechaHoraUtc,
                TipoAnterior = fichaje.Tipo,
                FechaHoraNuevaUtc = nuevaFechaHoraUtc,
                TipoNuevo = nuevoTipo,
                Motivo = motivo.Trim()
            };

            fichaje.FechaHoraUtc = nuevaFechaHoraUtc;
            fichaje.Tipo = nuevoTipo;

            _context.ModificacionesFichajes.Add(modificacion);

            await _context.SaveChangesAsync();

            return (true, "Fichaje modificado correctamente.");
        }
    }
}