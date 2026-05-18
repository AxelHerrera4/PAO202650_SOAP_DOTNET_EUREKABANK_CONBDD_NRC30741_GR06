using monster.edu.ec.modelo;
using monster.edu.ec.servicio;
using monster.edu.ec.vista;

namespace monster.edu.ec.controlador
{
    public class LoginController
    {
        private LoginView _view = new LoginView();
        private EurekaBankServiceClient _serviceClient;

        public LoginController(EurekaBankServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }

        public Usuario Autenticar()
        {
            _view.MostrarMenuLogin();
            string usuario = Console.ReadLine();

            Console.Write("Ingresa tu contraseña: ");
            string clave = Console.ReadLine();

            try
            {
                Usuario usuarioLogueado = _serviceClient.Login(usuario, clave);

                if (usuarioLogueado.Rol != "Superadmin Monster")
                {
                    _view.MostrarMensajeError("Acceso denegado. Solo el Superadmin Monster puede acceder.");
                    return null;
                }

                _view.MostrarMensajeExito($"Bienvenido {usuarioLogueado.Nombre}!");
                return usuarioLogueado;
            }
            catch (Exception ex)
            {
                _view.MostrarMensajeError(ex.Message);
                return null;
            }
        }
    }
}
