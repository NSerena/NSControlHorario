
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

        public async Task<IdentityResult> ActivarEmpleadoAsync(string id)
        {
            var empleado = await _userManager.FindByIdAsync(id);

            if (empleado == null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "Empleado no encontrado." });
            }

            empleado.Activo = true;

            return await _userManager.UpdateAsync(empleado);
        }

        public async Task<IdentityResult> ActualizarEmpleadoAsync(
            string id,
            string email,
            string nombre,
            string apellidos,
            string? dni,
            string? telefono)
        {
            var empleado = await _userManager.FindByIdAsync(id);

            if (empleado == null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "Empleado no encontrado." });
            }

            var usuarioConEmail = await _userManager.FindByEmailAsync(email);

            if (usuarioConEmail != null && usuarioConEmail.Id != id)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "Ya existe otro empleado con ese correo electrónico." });
            }

            var resultado = await _userManager.SetUserNameAsync(empleado, email);

            if (!resultado.Succeeded)
                return resultado;

            resultado = await _userManager.SetEmailAsync(empleado, email);

            if (!resultado.Succeeded)
                return resultado;

            empleado.Nombre = nombre;
            empleado.Apellidos = apellidos;
            empleado.DNI = dni;
            empleado.PhoneNumber = telefono;

            return await _userManager.UpdateAsync(empleado);
        }

        public async Task<IdentityResult> CambiarPasswordEmpleadoAsync(
            string id,
            string nuevaPassword)
        {
            var empleado = await _userManager.FindByIdAsync(id);

            if (empleado == null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "Empleado no encontrado." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(empleado);

            return await _userManager.ResetPasswordAsync(
                empleado,
                token,
                nuevaPassword);
        }

    }
}
