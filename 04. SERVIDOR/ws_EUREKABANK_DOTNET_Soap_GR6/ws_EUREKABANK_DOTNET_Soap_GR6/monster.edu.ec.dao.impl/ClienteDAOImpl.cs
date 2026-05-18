using monster.edu.ec.model;
using monster.edu.ec.util;
using Microsoft.Data.SqlClient;

namespace monster.edu.ec.dao.impl
{
    public class ClienteDAOImpl : IClienteDAO
    {
        public Cliente ValidarLogin(string dni)
        {
            Cliente cliente = null;
            string sql = "SELECT * FROM cliente WHERE dni = @dni";

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
                                cliente = new Cliente
                                {
                                    IdCliente = (int)reader["id_cliente"],
                                    Dni = (string)reader["dni"],
                                    Nombre = (string)reader["nombre"],
                                    Apellido = (string)reader["apellido"],
                                    Direccion = reader["direccion"].ToString(),
                                    Telefono = reader["telefono"].ToString(),
                                    Email = reader["email"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ValidarLogin: {ex.Message}");
            }

            return cliente;
        }
    }
}
