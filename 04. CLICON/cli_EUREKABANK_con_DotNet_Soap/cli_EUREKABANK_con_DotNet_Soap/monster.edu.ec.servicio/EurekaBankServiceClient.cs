using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using monster.edu.ec.modelo;
using ServiceReference1;

namespace monster.edu.ec.servicio
{
    public class EurekaBankServiceClient : IEurekaBankService
    {
        private EurekaBankWSControladorClient _client;
        private Usuario _usuarioActual;

        public EurekaBankServiceClient()
        {
            InitializeClient();
        }

        private void InitializeClient()
        {
            try
            {
                var binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = 2147483647;
                binding.MaxBufferSize = 2147483647;

                var endpoint = new EndpointAddress("http://192.168.1.14:8080/EurekaBankWS.asmx");
                _client = new EurekaBankWSControladorClient(binding, endpoint);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al inicializar cliente: {ex.Message}");
            }
        }

        public Usuario Login(string usuario, string clave)
        {
            try
            {
                if (_client == null)
                    throw new Exception("Cliente SOAP no inicializado");

                var response = _client.AutenticarUsuarioAsync(usuario, clave).Result;
                string resultado = response.Body.AutenticarUsuarioResult;

                if (resultado.StartsWith("SUCCESS"))
                {
                    var usuario_obj = new Usuario
                    {
                        Nombre = usuario,
                        Rol = ExtractRol(resultado),
                        NombreUsuario = usuario
                    };
                    _usuarioActual = usuario_obj;
                    return usuario_obj;
                }
                else
                {
                    throw new Exception(resultado.Replace("ERROR: ", ""));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en login: {ex.Message}");
            }
        }

        public string Depositar(string cuentaId, decimal monto, int empleadoId)
        {
            try
            {
                if (_client == null)
                    throw new Exception("Cliente SOAP no inicializado");

                var response = _client.DepositarAsync(cuentaId, (double)monto, empleadoId).Result;
                return response.Body.DepositarResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en deposito: {ex.Message}");
            }
        }

        public string Retirar(string cuentaId, decimal monto, int empleadoId)
        {
            try
            {
                if (_client == null)
                    throw new Exception("Cliente SOAP no inicializado");

                var response = _client.RetirarAsync(cuentaId, (double)monto, empleadoId).Result;
                return response.Body.RetirarResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en retiro: {ex.Message}");
            }
        }

        public string Transferir(string cuentaOrigen, string cuentaDestino, decimal monto, int empleadoId)
        {
            try
            {
                if (_client == null)
                    throw new Exception("Cliente SOAP no inicializado");

                var response = _client.TransferirAsync(cuentaOrigen, cuentaDestino, (double)monto, empleadoId).Result;
                return response.Body.TransferirResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en transferencia: {ex.Message}");
            }
        }

        public dynamic GetCuentaInfo(string dni)
        {
            try
            {
                if (_client == null)
                    throw new Exception("Cliente SOAP no inicializado");

                var response = _client.ConsultarCuentasPorClienteAsync(dni).Result;
                var result = response.Body.ConsultarCuentasPorClienteResult;

                if (result != null)
                {
                    return new
                    {
                        NumeroCuenta = result.NumeroCuenta,
                        Saldo = result.Saldo,
                        Disponibilidad = result.Disponibilidad,
                        NombreCliente = result.NombreCliente
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener cuenta: {ex.Message}");
            }
        }

        public List<dynamic> GetExtracto(string cuentaId)
        {
            try
            {
                if (_client == null)
                    throw new Exception("Cliente SOAP no inicializado");

                var response = _client.ConsultarExtractoAsync(cuentaId).Result;
                var movimientos = response.Body.ConsultarExtractoResult;
                var resultado = new List<dynamic>();

                if (movimientos != null)
                {
                    foreach (var m in movimientos)
                    {
                        resultado.Add(new
                        {
                            TipoMovimiento = m.Tipo,
                            Monto = m.Monto,
                            Fecha = m.FechaHora,
                            Empleado = m.NombreEmpleado,
                            NumeroOperacion = m.NumeroOperacion
                        });
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener extracto: {ex.Message}");
            }
        }

        public List<dynamic> ListarCuentasPorSucursal(int sucursalId)
        {
            try
            {
                if (_client == null)
                    throw new Exception("Cliente SOAP no inicializado");

                var cuentas = _client.ListarCuentasPorSucursalAsync(sucursalId).Result;
                var resultado = new List<dynamic>();

                if (cuentas != null)
                {
                    foreach (var c in cuentas)
                    {
                        resultado.Add(new
                        {
                            NumeroCuenta = c.NumeroCuenta,
                            Saldo = c.Saldo,
                            NombreCliente = c.NombreCliente,
                            ApellidoCliente = c.ApellidoCliente,
                            Disponibilidad = c.Disponibilidad
                        });
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al listar cuentas por sucursal: {ex.Message}");
            }
        }

        public Usuario GetUsuarioActual()
        {
            return _usuarioActual;
        }

        private string ExtractRol(string mensaje)
        {
            var match = System.Text.RegularExpressions.Regex.Match(mensaje, @"\(([^)]+)\)");
            return match.Success ? match.Groups[1].Value : "Cliente";
        }

        public void Logout()
        {
            _usuarioActual = null;
            if (_client != null)
            {
                try
                {
                    _client.Close();
                }
                catch { }
            }
        }
    }
}
