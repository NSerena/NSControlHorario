
using Microsoft.AspNetCore.Identity;
using NSControlHorarioApp.Models;

namespace NSControlHorarioApp.Services
{
    public class EmpleadoService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EmpleadoService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<ApplicationUser>> ObtenerEmpleadosAsync()
        {
            var usuarios = _userManager.Users.ToList();

            var empleados = new List<ApplicationUser>();

            foreach (var usuario in usuarios)
            {
                if (await _userManager.IsInRoleAsync(usuario, "Empleado"))
                {
                    empleados.Add(usuario);
                }
            }

            return empleados;
        }

        public async Task<IdentityResult> CrearEmpleadoAsync(
            string email,
            string password,
            string nombre,
            string apellidos,
            string dni)
        {
            var empleado = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Nombre = nombre,
                Apellidos = apellidos,
                DNI = dni,
                FechaAlta = DateTime.UtcNow,
                Activo = true,
                EmailConfirmed = true
            };

            var resultado = await _userManager.CreateAsync(empleado, password);

            if (resultado.Succeeded)
            {
                await _userManager.AddToRoleAsync(empleado, "Empleado");
            }

            return resultado;
        }

        public async Task<IdentityResult> DesactivarEmpleadoAsync(string id)
        {
            var empleado = await _userManager.FindByIdAsync(id);

            if (empleado == null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "Empleado no encontrado." });
            }

            empleado.Activo = false;

            return await _userManager.UpdateAsync(empleado);
        }
    }
}
