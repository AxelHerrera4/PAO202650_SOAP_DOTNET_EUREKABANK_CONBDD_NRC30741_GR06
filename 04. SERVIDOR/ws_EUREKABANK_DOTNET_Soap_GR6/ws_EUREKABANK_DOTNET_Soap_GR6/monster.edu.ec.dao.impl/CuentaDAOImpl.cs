using monster.edu.ec.model;
using monster.edu.ec.util;
using monster.edu.ec.servicios;
using Microsoft.Data.SqlClient;

namespace monster.edu.ec.dao.impl
{
    public class CuentaDAOImpl : ICuentaDAO
    {
        public Cuenta ObtenerCuenta(string numeroCuenta)
        {
            try
            {
                using (SqlConnection con = ConexionDB.GetConexion())
                {
                    return con != null ? ObtenerCuentaForUpdate(con, numeroCuenta) : null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerCuenta: {ex.Message}");
                return null;
            }
        }

        public Cuenta ObtenerCuentaForUpdate(SqlConnection con, string numeroCuenta)
        {
            return ObtenerCuentaForUpdate(con, numeroCuenta, null);
        }

        public Cuenta ObtenerCuentaForUpdate(SqlConnection con, string numeroCuenta, SqlTransaction transaction = null)
        {
            Cuenta cuenta = null;
            string sql = "SELECT * FROM cuenta WITH (UPDLOCK, ROWLOCK) WHERE numero_cuenta = @numeroCuenta";

            try
            {
                using (var cmd = new SqlCommand(sql, con))
                {
                    if (transaction != null)
                        cmd.Transaction = transaction;

                    cmd.Parameters.AddWithValue("@numeroCuenta", numeroCuenta);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cuenta = new Cuenta
                            {
                                NumeroCuenta = (string)reader["numero_cuenta"],
                                Saldo = (decimal)reader["saldo"],
                                IdCliente = (int)reader["id_cliente"],
                                IdSucursal = (int)reader["id_sucursal"],
                                Estado = (string)reader["estado"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerCuentaForUpdate: {ex.Message}");
            }

            return cuenta;
        }

        public bool ActualizarSaldo(SqlConnection con, string numeroCuenta, decimal nuevoSaldo)
        {
            return ActualizarSaldo(con, numeroCuenta, nuevoSaldo, null);
        }

        public bool ActualizarSaldo(SqlConnection con, string numeroCuenta, decimal nuevoSaldo, SqlTransaction transaction = null)
        {
            string sql = "UPDATE cuenta SET saldo = @saldo WHERE numero_cuenta = @numeroCuenta";

            try
            {
                using (var cmd = new SqlCommand(sql, con))
                {
                    if (transaction != null)
                        cmd.Transaction = transaction;

                    cmd.Parameters.AddWithValue("@saldo", nuevoSaldo);
                    cmd.Parameters.AddWithValue("@numeroCuenta", numeroCuenta);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarSaldo: {ex.Message}");
                return false;
            }
        }

        public bool ActualizarSaldo(string numeroCuenta, decimal nuevoSaldo)
        {
            try
            {
                using (SqlConnection con = ConexionDB.GetConexion())
                {
                    return con != null && ActualizarSaldo(con, numeroCuenta, nuevoSaldo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarSaldo: {ex.Message}");
                return false;
            }
        }

        public List<CuentaClienteDTO> ObtenerCuentasConClientes(int sucursalId)
        {
            List<CuentaClienteDTO> lista = new();
            string sql = "SELECT cu.numero_cuenta, cu.saldo, cu.id_cliente, cl.nombre, cl.apellido " +
                         "FROM cuenta cu " +
                         "INNER JOIN cliente cl ON cu.id_cliente = cl.id_cliente " +
                         "WHERE cu.id_sucursal = @sucursalId";

            try
            {
                using (SqlConnection con = ConexionDB.GetConexion())
                {
                    if (con == null) return lista;

                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@sucursalId", sucursalId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            var gestor = GestorConcurrencia.GetInstancia();
                            while (reader.Read())
                            {
                                string nroCuenta = (string)reader["numero_cuenta"];
                                var dto = new CuentaClienteDTO
                                {
                                    NumeroCuenta = nroCuenta,
                                    Saldo = (decimal)reader["saldo"],
                                    IdCliente = (int)reader["id_cliente"],
                                    NombreCliente = (string)reader["nombre"],
                                    ApellidoCliente = (string)reader["apellido"],
                                    Disponibilidad = gestor.GetEstadoCuenta(nroCuenta)
                                };
                                lista.Add(dto);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerCuentasConClientes: {ex.Message}");
            }

            return lista;
        }

        public CuentaClienteDTO ObtenerCuentaPorDni(string dni)
        {
            CuentaClienteDTO dto = null;
            string sql = "SELECT cu.numero_cuenta, cu.saldo, cu.id_cliente, cl.nombre, cl.apellido " +
                         "FROM cuenta cu " +
                         "INNER JOIN cliente cl ON cu.id_cliente = cl.id_cliente " +
                         "WHERE cl.dni = @dni";

            try
            {
                using (SqlConnection con = ConexionDB.GetConexion())
                {
                    if (con == null) return null;

                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@dni", dni);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string nroCuenta = (string)reader["numero_cuenta"];
                                var gestor = GestorConcurrencia.GetInstancia();
                                dto = new CuentaClienteDTO
                                {
                                    NumeroCuenta = nroCuenta,
                                    Saldo = (decimal)reader["saldo"],
                                    IdCliente = (int)reader["id_cliente"],
                                    NombreCliente = (string)reader["nombre"],
                                    ApellidoCliente = (string)reader["apellido"],
                                    Disponibilidad = gestor.GetEstadoCuenta(nroCuenta)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerCuentaPorDni: {ex.Message}");
            }

            return dto;
        }
    }
}
