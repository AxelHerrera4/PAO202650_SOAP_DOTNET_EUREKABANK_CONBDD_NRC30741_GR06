using monster.edu.ec.model;
using monster.edu.ec.servicios;
using System.ServiceModel;

namespace monster.edu.ec.vista
{
    [ServiceContract]
    public class EurekaBankWS
    {
        private readonly SeguridadService _seguridadService;
        private readonly TransaccionService _transaccionService;

        public EurekaBankWS()
        {
            _seguridadService = new SeguridadService();
            _transaccionService = new TransaccionService();
        }

        [OperationContract]
        public string AutenticarUsuario(string usuario, string clave)
        {
            try
            {
                var emp = _seguridadService.Login(usuario, clave);
                if (emp != null)
                {
                    return $"SUCCESS: Bienvenido {emp.NombreCompleto} ({emp.Rol})";
                }

                var cli = _seguridadService.LoginCliente(usuario);
                if (cli != null)
                {
                    return $"SUCCESS: Bienvenido {cli.Nombre} {cli.Apellido} (Cliente)";
                }

                return "ERROR: Credenciales inválidas o DNI no registrado.";
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }

        [OperationContract]
        public string Depositar(string cuentaId, double monto, int empleadoId)
        {
            try
            {
                string operacion = _transaccionService.RealizarDeposito(cuentaId, (decimal)monto, empleadoId);
                return $"SUCCESS: Depósito realizado. Nro Operación: {operacion}";
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }

        [OperationContract]
        public string Retirar(string cuentaId, double monto, int empleadoId)
        {
            try
            {
                string operacion = _transaccionService.RealizarRetiro(cuentaId, (decimal)monto, empleadoId);
                return $"SUCCESS: Retiro realizado. Nro Operación: {operacion}";
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }

        [OperationContract]
        public string Transferir(string cuentaOrigenId, string cuentaDestinoId, double monto, int empleadoId)
        {
            try
            {
                string operacion = _transaccionService.RealizarTransferencia(cuentaOrigenId, cuentaDestinoId, (decimal)monto, empleadoId);
                return $"SUCCESS: Transferencia exitosa. Nro Operación: {operacion}";
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }

        [OperationContract]
        public List<CuentaClienteDTO> ListarCuentasPorSucursal(int sucursalId)
        {
            try
            {
                return _transaccionService.ObtenerCuentasSucursal(sucursalId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                return new List<CuentaClienteDTO>();
            }
        }

        [OperationContract]
        public List<Movimiento> ConsultarExtracto(string cuentaId)
        {
            try
            {
                return _transaccionService.ObtenerExtracto(cuentaId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                return new List<Movimiento>();
            }
        }

        [OperationContract]
        public CuentaClienteDTO ConsultarCuentasPorCliente(string dni)
        {
            try
            {
                return _transaccionService.ObtenerCuentaClientePorDni(dni);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                return null;
            }
        }
    }
}
