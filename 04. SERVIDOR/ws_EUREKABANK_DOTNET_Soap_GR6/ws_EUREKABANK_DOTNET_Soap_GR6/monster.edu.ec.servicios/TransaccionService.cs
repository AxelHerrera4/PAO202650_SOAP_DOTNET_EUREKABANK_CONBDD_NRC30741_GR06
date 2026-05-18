using monster.edu.ec.dao;
using monster.edu.ec.dao.impl;
using monster.edu.ec.model;
using monster.edu.ec.util;

namespace monster.edu.ec.servicios
{
    public class TransaccionService
    {
        private readonly ICuentaDAO _cuentaDAO;
        private readonly IMovimientoDAO _movimientoDAO;
        private readonly GestorConcurrencia _gestorConcurrencia;

        public TransaccionService()
        {
            _cuentaDAO = new CuentaDAOImpl();
            _movimientoDAO = new MovimientoDAOImpl();
            _gestorConcurrencia = GestorConcurrencia.GetInstancia();
        }

        public string RealizarDeposito(string numeroCuenta, decimal monto, int idEmpleado)
        {
            if (!_gestorConcurrencia.BloquearCuenta(numeroCuenta))
                throw new Exception("CONCURRENCIA_ERROR: Cuenta ocupada.");

            try
            {
                using (var con = ConexionDB.GetConexion())
                {
                    if (con == null) throw new Exception("No se pudo conectar a la BD.");

                    using (var transaccion = con.BeginTransaction())
                    {
                        try
                        {
                            var cuenta = _cuentaDAO.ObtenerCuentaForUpdate(con, numeroCuenta, transaccion);
                            if (cuenta == null) throw new Exception("Cuenta no existe.");

                            Console.WriteLine($"🔒 [MASHUP] Cuenta {numeroCuenta} bloqueada. Simulando latencia bancaria de 5 segundos...");
                            Thread.Sleep(5000);
                            Console.WriteLine("🔓 Candado liberado, finalizando transacción...");

                            decimal nuevoSaldo = cuenta.Saldo + monto;
                            _cuentaDAO.ActualizarSaldo(con, numeroCuenta, nuevoSaldo, transaccion);

                            string nroOp = $"DEP-{DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
                            _movimientoDAO.RegistrarMovimiento(con, new Movimiento
                            {
                                NumeroOperacion = nroOp,
                                NumeroCuenta = numeroCuenta,
                                IdEmpleado = idEmpleado,
                                Tipo = "Deposito",
                                Monto = monto
                            }, transaccion);

                            transaccion.Commit();
                            return nroOp;
                        }
                        catch (Exception ex)
                        {
                            transaccion.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            finally
            {
                _gestorConcurrencia.LiberarCuenta(numeroCuenta);
            }
        }

        public string RealizarRetiro(string numeroCuenta, decimal monto, int idEmpleado)
        {
            if (!_gestorConcurrencia.BloquearCuenta(numeroCuenta))
                throw new Exception("CONCURRENCIA_ERROR: Cuenta ocupada.");

            try
            {
                using (var con = ConexionDB.GetConexion())
                {
                    if (con == null) throw new Exception("No se pudo conectar a la BD.");

                    using (var transaccion = con.BeginTransaction())
                    {
                        try
                        {
                            var cuenta = _cuentaDAO.ObtenerCuentaForUpdate(con, numeroCuenta, transaccion);
                            if (cuenta == null) throw new Exception("Cuenta no existe.");
                            if (cuenta.Saldo < monto) throw new Exception("SALDO_INSUFICIENTE.");

                            Console.WriteLine($"🔒 [MASHUP] Cuenta {numeroCuenta} bloqueada. Simulando latencia bancaria de 5 segundos...");
                            Thread.Sleep(5000);
                            Console.WriteLine("🔓 Candado liberado, finalizando transacción...");

                            decimal nuevoSaldo = cuenta.Saldo - monto;
                            _cuentaDAO.ActualizarSaldo(con, numeroCuenta, nuevoSaldo, transaccion);

                            string nroOp = $"RET-{DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
                            _movimientoDAO.RegistrarMovimiento(con, new Movimiento
                            {
                                NumeroOperacion = nroOp,
                                NumeroCuenta = numeroCuenta,
                                IdEmpleado = idEmpleado,
                                Tipo = "Retiro",
                                Monto = monto
                            }, transaccion);

                            transaccion.Commit();
                            return nroOp;
                        }
                        catch (Exception ex)
                        {
                            transaccion.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            finally
            {
                _gestorConcurrencia.LiberarCuenta(numeroCuenta);
            }
        }

        public string RealizarTransferencia(string cuentaOrigen, string cuentaDestino, decimal monto, int idEmpleado)
        {
            string first = string.Compare(cuentaOrigen, cuentaDestino) < 0 ? cuentaOrigen : cuentaDestino;
            string second = first == cuentaOrigen ? cuentaDestino : cuentaOrigen;

            if (!_gestorConcurrencia.BloquearCuenta(first))
                throw new Exception($"CONCURRENCIA_ERROR: {first}");

            try
            {
                if (!_gestorConcurrencia.BloquearCuenta(second))
                    throw new Exception($"CONCURRENCIA_ERROR: {second}");

                try
                {
                    using (var con = ConexionDB.GetConexion())
                    {
                        if (con == null) throw new Exception("No se pudo conectar a la BD.");

                        using (var transaccion = con.BeginTransaction())
                        {
                            try
                            {
                                var cOrig = _cuentaDAO.ObtenerCuentaForUpdate(con, cuentaOrigen, transaccion);
                                var cDest = _cuentaDAO.ObtenerCuentaForUpdate(con, cuentaDestino, transaccion);

                                if (cOrig == null || cDest == null)
                                    throw new Exception("Una de las cuentas no existe.");
                                if (cOrig.Saldo < monto)
                                    throw new Exception("SALDO_INSUFICIENTE.");

                                Console.WriteLine($"🔒 [MASHUP] Cuenta {cuentaOrigen} bloqueada. Simulando latencia bancaria de 5 segundos...");
                                Thread.Sleep(5000);
                                Console.WriteLine("🔓 Candado liberado, finalizando transacción...");

                                _cuentaDAO.ActualizarSaldo(con, cuentaOrigen, cOrig.Saldo - monto, transaccion);
                                _cuentaDAO.ActualizarSaldo(con, cuentaDestino, cDest.Saldo + monto, transaccion);

                                string nroBase = $"TRA-{DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
                                _movimientoDAO.RegistrarMovimiento(con, new Movimiento
                                {
                                    NumeroOperacion = $"{nroBase}-R",
                                    NumeroCuenta = cuentaOrigen,
                                    IdEmpleado = idEmpleado,
                                    Tipo = "Retiro",
                                    Monto = monto
                                }, transaccion);

                                _movimientoDAO.RegistrarMovimiento(con, new Movimiento
                                {
                                    NumeroOperacion = $"{nroBase}-D",
                                    NumeroCuenta = cuentaDestino,
                                    IdEmpleado = idEmpleado,
                                    Tipo = "Deposito",
                                    Monto = monto
                                }, transaccion);

                                transaccion.Commit();
                                return nroBase;
                            }
                            catch (Exception ex)
                            {
                                transaccion.Rollback();
                                throw ex;
                            }
                        }
                    }
                }
                finally
                {
                    _gestorConcurrencia.LiberarCuenta(second);
                }
            }
            finally
            {
                _gestorConcurrencia.LiberarCuenta(first);
            }
        }

        public List<CuentaClienteDTO> ObtenerCuentasSucursal(int sucursalId)
        {
            return _cuentaDAO.ObtenerCuentasConClientes(sucursalId);
        }

        public List<Movimiento> ObtenerExtracto(string numeroCuenta)
        {
            return _movimientoDAO.ListarExtracto(numeroCuenta);
        }

        public CuentaClienteDTO ObtenerCuentaClientePorDni(string dni)
        {
            return _cuentaDAO.ObtenerCuentaPorDni(dni);
        }
    }
}
