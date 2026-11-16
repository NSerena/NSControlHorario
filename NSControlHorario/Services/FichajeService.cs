
using Microsoft.EntityFrameworkCore;
using NSControlHorario.Data;
using NSControlHorarioApp.Models;

namespace NSControlHorarioApp.Services
{
    public class FichajeService
    {
        private readonly ApplicationDbContext _context;

        public FichajeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Fichaje?> ObtenerUltimoFichajeAsync(string userId)
        {
            return await _context.Fichajes
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.FechaHoraUtc)
                .ThenByDescending(f => f.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<(bool Correcto, string Mensaje)> RegistrarFichajeAsync(
            string userId,
            TipoFichaje tipo)
        {
            var ultimo = await ObtenerUltimoFichajeAsync(userId);

            // Si no hay fichajes o el último fue una salida,
            // el siguiente debe ser una entrada.
            if (ultimo == null || ultimo.Tipo == TipoFichaje.Salida)
            {
                if (tipo != TipoFichaje.Entrada)
                    return (false, "Debes registrar primero una entrada.");

            }
            else if (ultimo.Tipo == TipoFichaje.Entrada)
            {
                if (tipo != TipoFichaje.Salida)
                    return (false, "Ya tienes una entrada registrada. Debes registrar una salida.");
            }

            var fichaje = new Fichaje
            {
                UserId = userId,
                Tipo = tipo,
                FechaHoraUtc = DateTime.UtcNow
            };

            _context.Fichajes.Add(fichaje);
            await _context.SaveChangesAsync();

            return (true, "Fichaje registrado correctamente.");
        }
    }
}
