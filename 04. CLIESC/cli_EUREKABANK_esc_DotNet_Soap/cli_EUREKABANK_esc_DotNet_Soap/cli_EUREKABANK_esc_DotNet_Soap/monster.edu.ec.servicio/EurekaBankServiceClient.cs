using System;

namespace cli_EUREKABANK_esc_DotNet_Soap.monster.edu.ec.servicio
{
    /// <summary>
    /// Cliente de servicio para Eureka Bank
    /// Encapsula la comunicación con el Web Service SOAP
    /// </summary>
    public class EurekaBankServiceClient
    {
        private readonly EurekaBankServiceProxy _proxy;

        public EurekaBankServiceClient()
        {
            _proxy = new EurekaBankServiceProxy();
        }

        /// <summary>
        /// Valida las credenciales del usuario
        /// </summary>
        /// <param name="usuario">Nombre de usuario</param>
        /// <param name="clave">Contraseña</param>
        /// <returns>ID del empleado si es válido, 0 en caso contrario</returns>
        public int ValidarCredenciales(string usuario, string clave)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave))
                return 0;

            return _proxy.ValidarCredenciales(usuario, clave);
        }

        /// <summary>
        /// Obtiene el nombre completo de un empleado
        /// </summary>
        /// <param name="idEmpleado">ID del empleado</param>
        /// <returns>Nombre completo del empleado</returns>
        public string ObtenerNombreEmpleado(int idEmpleado)
        {
            if (idEmpleado <= 0)
                return "Usuario";

            return _proxy.ObtenerNombreEmpleado(idEmpleado);
        }
    }
}
