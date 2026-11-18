namespace NSControlHorarioApp.Services
{
    public class NotificacionService
    {
        public event Action<string, string>? NotificacionSolicitada;

        public void Mostrar(string mensaje, string tipo = "info")
        {
            NotificacionSolicitada?.Invoke(mensaje, tipo); //success = verde | error = rojo | warning = amarillo | info = azul
        }
    }
}