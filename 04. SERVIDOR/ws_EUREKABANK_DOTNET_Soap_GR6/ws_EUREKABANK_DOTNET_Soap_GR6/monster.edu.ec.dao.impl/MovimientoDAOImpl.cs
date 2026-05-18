using monster.edu.ec.model;
using monster.edu.ec.util;
using Microsoft.Data.SqlClient;

namespace monster.edu.ec.dao.impl
{
    public class MovimientoDAOImpl : IMovimientoDAO
    {
        public bool RegistrarMovimiento(Movimiento movimiento)
        {
            try
            {
                using (SqlConnection con = ConexionDB.GetConexion())
                {
                    return con != null && RegistrarMovimiento(con, movimiento);
                }
            }
            catch
            {
                return false;
            }
        }

        public bool RegistrarMovimiento(SqlConnection con, Movimiento movimiento)
        {
            return RegistrarMovimiento(con, movimiento, null);
        }

        public bool RegistrarMovimiento(SqlConnection con, Movimiento movimiento, SqlTransaction transaction = null)
        {
            string sql = "INSERT INTO movimiento (numero_operacion, numero_cuenta, id_empleado, tipo, monto) " +
                         "VALUES (@numeroOperacion, @numeroCuenta, @idEmpleado, @tipo, @monto)";

            try
            {
                using (var cmd = new SqlCommand(sql, con))
                {
                    if (transaction != null)
                        cmd.Transaction = transaction;

                    cmd.Parameters.AddWithValue("@numeroOperacion", movimiento.NumeroOperacion);
                    cmd.Parameters.AddWithValue("@numeroCuenta", movimiento.NumeroCuenta);
                    cmd.Parameters.AddWithValue("@idEmpleado", movimiento.IdEmpleado);
                    cmd.Parameters.AddWithValue("@tipo", movimiento.Tipo);
                    cmd.Parameters.AddWithValue("@monto", movimiento.Monto);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en RegistrarMovimiento: {ex.Message}");
                return false;
            }
        }

        public List<Movimiento> ListarExtracto(string numeroCuenta)
        {
            List<Movimiento> movimientos = new();
            string sql = "SELECT m.*, e.nombre_completo FROM movimiento m " +
                         "JOIN empleado e ON m.id_empleado = e.id_empleado " +
                         "WHERE m.numero_cuenta = @numeroCuenta " +
                         "ORDER BY m.fecha_hora DESC";

            try
            {
                using (SqlConnection con = ConexionDB.GetConexion())
                {
                    if (con == null) return movimientos;

                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@numeroCuenta", numeroCuenta);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var mov = new Movimiento
                                {
                                    IdMovimiento = (int)reader["id_movimiento"],
                                    NumeroOperacion = (string)reader["numero_operacion"],
                                    NumeroCuenta = (string)reader["numero_cuenta"],
                                    IdEmpleado = (int)reader["id_empleado"],
                                    NombreEmpleado = (string)reader["nombre_completo"],
                                    Tipo = (string)reader["tipo"],
                                    Monto = (decimal)reader["monto"],
                                    FechaHora = reader["fecha_hora"] != DBNull.Value
                                        ? ((DateTime)reader["fecha_hora"]).ToString("dd/MM/yyyy HH:mm:ss")
                                        : null
                                };
                                movimientos.Add(mov);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ListarExtracto: {ex.Message}");
            }

            return movimientos;
        }
    }
}
