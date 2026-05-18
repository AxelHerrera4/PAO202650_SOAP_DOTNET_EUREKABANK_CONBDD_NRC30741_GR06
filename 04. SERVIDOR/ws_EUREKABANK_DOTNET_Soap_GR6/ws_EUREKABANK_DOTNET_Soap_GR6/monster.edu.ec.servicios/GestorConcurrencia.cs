using System.Collections.Concurrent;

namespace monster.edu.ec.servicios
{
    public class GestorConcurrencia
    {
        private static GestorConcurrencia _instancia;
        private static readonly object _lock = new object();
        private readonly ConcurrentDictionary<string, string> _estadosCuentas;

        private const string VERDE = "VERDE";
        private const string ROJO = "ROJO";

        private GestorConcurrencia()
        {
            _estadosCuentas = new ConcurrentDictionary<string, string>();
        }

        public static GestorConcurrencia GetInstancia()
        {
            if (_instancia == null)
            {
                lock (_lock)
                {
                    if (_instancia == null)
                    {
                        _instancia = new GestorConcurrencia();
                    }
                }
            }
            return _instancia;
        }

        public bool BloquearCuenta(string numeroCuenta)
        {
            lock (_lock)
            {
                _estadosCuentas.TryGetValue(numeroCuenta, out var estadoActual);
                if (estadoActual == null) estadoActual = VERDE;

                if (estadoActual == VERDE)
                {
                    _estadosCuentas[numeroCuenta] = ROJO;
                    Console.WriteLine($"[Concurrencia] Cuenta {numeroCuenta} bloqueada (ROJO).");
                    return true;
                }

                Console.WriteLine($"[Concurrencia] Intento fallido: Cuenta {numeroCuenta} ya está ocupada (ROJO).");
                return false;
            }
        }

        public void LiberarCuenta(string numeroCuenta)
        {
            _estadosCuentas[numeroCuenta] = VERDE;
            Console.WriteLine($"[Concurrencia] Cuenta {numeroCuenta} liberada (VERDE).");
        }

        public string GetEstadoCuenta(string numeroCuenta)
        {
            _estadosCuentas.TryGetValue(numeroCuenta, out var estado);
            return estado ?? VERDE;
        }
    }
}
