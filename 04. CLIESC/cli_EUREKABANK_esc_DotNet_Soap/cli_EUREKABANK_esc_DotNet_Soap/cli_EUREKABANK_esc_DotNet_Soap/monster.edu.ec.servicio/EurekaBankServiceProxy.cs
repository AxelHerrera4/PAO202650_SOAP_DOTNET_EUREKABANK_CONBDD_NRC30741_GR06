using System;

namespace cli_EUREKABANK_esc_DotNet_Soap.monster.edu.ec.servicio
{
    /// <summary>
    /// Cliente proxy para consumir el Web Service SOAP de Eureka Bank
    /// URL: http://192.168.1.14:8080/EurekaBankWS.asmx
    /// </summary>
    public class EurekaBankServiceProxy
    {
        private readonly string _serviceUrl = "http://192.168.1.14:8080/EurekaBankWS.asmx";

        /// <summary>
        /// Valida las credenciales del usuario contra el servicio web
        /// </summary>
        /// <param name="usuario">Nombre de usuario</param>
        /// <param name="clave">Contraseña del usuario</param>
        /// <returns>ID del empleado si es válido, 0 si no</returns>
        public int ValidarCredenciales(string usuario, string clave)
        {
            try
            {
                // Crear la solicitud SOAP manualmente
                string soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tns=""http://EurekaBankWS/"">
    <soap:Body>
        <tns:ValidarCredenciales>
            <tns:usuario>{EscapeXml(usuario)}</tns:usuario>
            <tns:clave>{EscapeXml(clave)}</tns:clave>
        </tns:ValidarCredenciales>
    </soap:Body>
</soap:Envelope>";

                string soapResponse = EnviarSolicitudSOAP("ValidarCredenciales", soapRequest);

                // Parsear la respuesta SOAP
                int idEmp = ExtraerIdEmpleadoDelSOAP(soapResponse);
                return idEmp;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ValidarCredenciales: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Obtiene el nombre completo del empleado desde el servicio web
        /// </summary>
        /// <param name="idEmpleado">ID del empleado</param>
        /// <returns>Nombre completo del empleado</returns>
        public string ObtenerNombreEmpleado(int idEmpleado)
        {
            try
            {
                string soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tns=""http://EurekaBankWS/"">
    <soap:Body>
        <tns:ObtenerNombreEmpleado>
            <tns:idEmpleado>{idEmpleado}</tns:idEmpleado>
        </tns:ObtenerNombreEmpleado>
    </soap:Body>
</soap:Envelope>";

                string soapResponse = EnviarSolicitudSOAP("ObtenerNombreEmpleado", soapRequest);

                // Parsear la respuesta SOAP
                string nombre = ExtraerNombreDelSOAP(soapResponse);
                return string.IsNullOrWhiteSpace(nombre) ? "Usuario" : nombre;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerNombreEmpleado: {ex.Message}");
                return "Usuario";
            }
        }

        /// <summary>
        /// Envía una solicitud SOAP al servicio web
        /// </summary>
        private string EnviarSolicitudSOAP(string metodo, string soapRequest)
        {
            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(_serviceUrl);
                request.Method = "POST";
                request.ContentType = "text/xml; charset=utf-8";
                request.Headers.Add("SOAPAction", $"http://EurekaBankWS/{metodo}");
                request.Timeout = 10000; // 10 segundos

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(soapRequest);
                    writer.Flush();
                }

                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al enviar solicitud SOAP: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Extrae el ID del empleado de la respuesta SOAP
        /// </summary>
        private int ExtraerIdEmpleadoDelSOAP(string soapResponse)
        {
            try
            {
                // Buscar el patrón en la respuesta
                // Puede variar según la estructura del servicio
                // Ejemplos: <id>123</id> o <idEmpleado>123</idEmpleado>

                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(soapResponse);

                // Definir namespaces
                System.Xml.XmlNamespaceManager nsm = new System.Xml.XmlNamespaceManager(doc.NameTable);
                nsm.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                nsm.AddNamespace("tns", "http://EurekaBankWS/");

                // Intentar obtener el valor de diferentes posibles nodos
                System.Xml.XmlNode node = doc.SelectSingleNode("//tns:ValidarCredencialesResult", nsm) 
                    ?? doc.SelectSingleNode("//id_empleado") 
                    ?? doc.SelectSingleNode("//idEmpleado");

                if (node != null && int.TryParse(node.InnerText, out int id))
                {
                    return id;
                }

                return 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al parsear respuesta SOAP: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Extrae el nombre del empleado de la respuesta SOAP
        /// </summary>
        private string ExtraerNombreDelSOAP(string soapResponse)
        {
            try
            {
                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(soapResponse);

                // Definir namespaces
                System.Xml.XmlNamespaceManager nsm = new System.Xml.XmlNamespaceManager(doc.NameTable);
                nsm.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                nsm.AddNamespace("tns", "http://EurekaBankWS/");

                // Intentar obtener el valor de diferentes posibles nodos
                System.Xml.XmlNode node = doc.SelectSingleNode("//tns:ObtenerNombreEmpleadoResult", nsm)
                    ?? doc.SelectSingleNode("//nombre_completo")
                    ?? doc.SelectSingleNode("//nombre");

                if (node != null)
                {
                    return node.InnerText;
                }

                return "Usuario";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al parsear respuesta SOAP: {ex.Message}");
                return "Usuario";
            }
        }

        /// <summary>
        /// Escapa caracteres especiales XML
        /// </summary>
        private string EscapeXml(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return System.Security.SecurityElement.Escape(text);
        }
    }
}
