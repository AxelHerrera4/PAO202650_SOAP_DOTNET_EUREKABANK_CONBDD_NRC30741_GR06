using monster.edu.ec.model;
using monster.edu.ec.util;
using Microsoft.Data.SqlClient;

namespace monster.edu.ec.dao.impl
{
    public class EmpleadoDAOImpl : IEmpleadoDAO
    {
        public Empleado ValidarLogin(string usuario, string password)
        {
            Empleado empleado = null;
            string sql = "SELECT * FROM empleado WHERE usuario = @usuario";

            try
            {
                using (SqlConnection con = ConexionDB.GetConexion())
                {
                    if (con == null) return null;

                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@usuario", usuario);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string hashedPassword = reader["password"].ToString();

                                if (SeguridadUtil.CheckPassword(password, hashedPassword))
                                {
                                    empleado = new Empleado
                                    {
                                        IdEmpleado = (int)reader["id_empleado"],
                                        Usuario = (string)reader["usuario"],
                                        Password = hashedPassword,
                                        NombreCompleto = (string)reader["nombre_completo"],
                                        Rol = (string)reader["rol"]
                                    };
                                }
                                else
                                {
                                    Console.WriteLine($"[Seguridad] Intento fallido: Contraseña incorrecta para {usuario}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"[Seguridad] Intento fallido: Usuario inexistente {usuario}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ValidarLogin: {ex.Message}");
            }

            return empleado;
        }
    }
}
