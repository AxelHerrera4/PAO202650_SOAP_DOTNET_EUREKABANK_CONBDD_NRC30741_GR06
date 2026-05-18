using monster.edu.ec.modelo;
using monster.edu.ec.servicio;
using monster.edu.ec.vista;
using monster.edu.ec.controlador;

class Program
{
    private static EurekaBankServiceClient _serviceClient;
    private static Usuario _usuarioLogueado;

    static async Task Main(string[] args)
    {
        _serviceClient = new EurekaBankServiceClient();
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        bool salir = false;
        while (!salir)
        {
            if (_usuarioLogueado == null)
            {
                LoginController loginController = new LoginController(_serviceClient);
                _usuarioLogueado = loginController.Autenticar();
            }
            else
            {
                MenuController menuController = new MenuController(_serviceClient);
                salir = menuController.MostrarMenu(_usuarioLogueado);
                if (salir)
                {
                    _usuarioLogueado = null;
                }
            }
        }

        Console.WriteLine("\nHasta luego!");
    }
}
