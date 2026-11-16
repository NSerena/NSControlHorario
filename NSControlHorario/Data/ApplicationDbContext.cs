
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NSControlHorarioApp.Models;

namespace NSControlHorario.Data
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Fichaje> Fichajes => Set<Fichaje>();
        public DbSet<ModificacionFichaje> ModificacionesFichajes => Set<ModificacionFichaje>();
        public DbSet<Ausencia> Ausencias { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ModificacionFichaje>()
                .HasOne(m => m.Fichaje)
                .WithMany()
                .HasForeignKey(m => m.FichajeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ModificacionFichaje>()
                .HasOne(m => m.UsuarioModificacion)
                .WithMany()
                .HasForeignKey(m => m.UsuarioModificacionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Ausencia>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Ausencia>()
                .HasOne(a => a.UsuarioRegistro)
                .WithMany()
                .HasForeignKey(a => a.UsuarioRegistroId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
