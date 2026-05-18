using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace cli_EUREKABANK_esc_DotNet_Soap.monster.edu.ec.servicio
{
    public class CuentaClienteDTO
    {
        public string NumeroCuenta { get; set; }
        public decimal Saldo { get; set; }
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidoCliente { get; set; }
        public string Disponibilidad { get; set; }
    }

    public class Movimiento
    {
        public int IdMovimiento { get; set; }
        public string NumeroOperacion { get; set; }
        public string NumeroCuenta { get; set; }
        public int IdEmpleado { get; set; }
        public string NombreEmpleado { get; set; }
        public string Tipo { get; set; }
        public decimal Monto { get; set; }
        public string FechaHora { get; set; }
    }

    public class EurekaBankServiceClient
    {
        private const string WS_URL = "http://192.168.0.101:8080/EurekaBankWS.asmx";
        private const string SOAP_NS = "http://schemas.xmlsoap.org/soap/envelope/";
        private const string SERVICE_NS = "http://tempuri.org/";
        private const string SERVICE_ACTION = "http://tempuri.org/EurekaBankWSControlador";
        private static readonly string LOG_FILE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "soap_debug.log");

        private static void LogDebug(string message)
        {
            try
            {
                File.AppendAllText(LOG_FILE, $"[{DateTime.Now:HH:mm:ss.fff}] {message}\n");
            }
            catch { }
        }

        public string AutenticarUsuario(string usuario, string clave)
        {
            try
            {
                string soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tns=""http://tempuri.org/"">
  <soap:Body>
    <tns:AutenticarUsuario>
      <usuario>{XmlEscape(usuario)}</usuario>
      <clave>{XmlEscape(clave)}</clave>
    </tns:AutenticarUsuario>
  </soap:Body>
</soap:Envelope>";

                return ExecuteSoapRequest("AutenticarUsuario", soapRequest);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al autenticar: " + ex.Message, ex);
            }
        }

        public List<CuentaClienteDTO> ListarCuentasPorSucursal(int sucursalId)
        {
            try
            {
                string soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tns=""http://tempuri.org/"">
  <soap:Body>
    <tns:ListarCuentasPorSucursal>
      <sucursalId>{sucursalId}</sucursalId>
    </tns:ListarCuentasPorSucursal>
  </soap:Body>
</soap:Envelope>";

                string response = ExecuteSoapRequest("ListarCuentasPorSucursal", soapRequest);
                return ParseCuentaClienteDTOList(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return new List<CuentaClienteDTO>();
            }
        }

        public List<Movimiento> ConsultarExtracto(string cuentaId)
        {
            try
            {
                string soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tns=""http://tempuri.org/"">
  <soap:Body>
    <tns:ConsultarExtracto>
      <cuentaId>{XmlEscape(cuentaId)}</cuentaId>
    </tns:ConsultarExtracto>
  </soap:Body>
</soap:Envelope>";

                string response = ExecuteSoapRequest("ConsultarExtracto", soapRequest);
                return ParseMovimientoList(response);
            }
            catch (Exception ex)
            {
                LogDebug("Error: " + ex.Message);
                return new List<Movimiento>();
            }
        }

        public string Depositar(string cuentaId, double monto, int empleadoId)
        {
            try
            {
                string soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tns=""http://tempuri.org/"">
  <soap:Body>
    <tns:Depositar>
      <cuentaId>{XmlEscape(cuentaId)}</cuentaId>
      <monto>{monto}</monto>
      <empleadoId>{empleadoId}</empleadoId>
    </tns:Depositar>
  </soap:Body>
</soap:Envelope>";

                return ExecuteSoapRequest("Depositar", soapRequest);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al depositar: " + ex.Message, ex);
            }
        }

        public string Retirar(string cuentaId, double monto, int empleadoId)
        {
            try
            {
                string soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tns=""http://tempuri.org/"">
  <soap:Body>
    <tns:Retirar>
      <cuentaId>{XmlEscape(cuentaId)}</cuentaId>
      <monto>{monto}</monto>
      <empleadoId>{empleadoId}</empleadoId>
    </tns:Retirar>
  </soap:Body>
</soap:Envelope>";

                return ExecuteSoapRequest("Retirar", soapRequest);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al retirar: " + ex.Message, ex);
            }
        }

        public string Transferir(string cuentaOrigenId, string cuentaDestinoId, double monto, int empleadoId)
        {
            try
            {
                string soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tns=""http://tempuri.org/"">
  <soap:Body>
    <tns:Transferir>
      <cuentaOrigenId>{XmlEscape(cuentaOrigenId)}</cuentaOrigenId>
      <cuentaDestinoId>{XmlEscape(cuentaDestinoId)}</cuentaDestinoId>
      <monto>{monto}</monto>
      <empleadoId>{empleadoId}</empleadoId>
    </tns:Transferir>
  </soap:Body>
</soap:Envelope>";

                return ExecuteSoapRequest("Transferir", soapRequest);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al transferir: " + ex.Message, ex);
            }
        }

        private string ExecuteSoapRequest(string method, string soapRequest)
        {
            try
            {
                string soapAction = $"{SERVICE_ACTION}/{method}";
                LogDebug($"Método: {method}");
                LogDebug($"SOAPAction: {soapAction}");
                LogDebug($"URL: {WS_URL}");
                LogDebug($"Request SOAP:\n{soapRequest}");

                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("Content-Type", "text/xml; charset=utf-8");
                    client.Headers.Add("SOAPAction", soapAction);

                    byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(soapRequest);
                    LogDebug($"Request enviado ({requestBytes.Length} bytes)");

                    byte[] responseBytes = client.UploadData(WS_URL, "POST", requestBytes);
                    string response = System.Text.Encoding.UTF8.GetString(responseBytes);

                    LogDebug($"Respuesta recibida ({response.Length} caracteres)");

                    return ExtractSoapResponse(response, method);
                }
            }
            catch (Exception ex)
            {
                LogDebug($"ERROR {ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null)
                    LogDebug($"InnerException: {ex.InnerException.Message}");
                throw new Exception("Error en SOAP: " + ex.Message, ex);
            }
        }

        private string ExtractSoapResponse(string soapResponse, string method)
        {
            try
            {
                LogDebug($"Respuesta completa: {soapResponse}");

                XDocument doc = XDocument.Parse(soapResponse);
                XNamespace soap = SOAP_NS;
                XNamespace tns = SERVICE_NS;

                var body = doc.Root.Element(soap + "Body");
                if (body == null) return "ERROR: Respuesta SOAP inválida";

                var response = body.Element(tns + method + "Response");
                if (response == null) response = body.Element(XName.Get(method + "Response"));

                if (response != null)
                {
                    var result = response.Element(tns + method + "Result") ??
                                response.Element(XName.Get(method + "Result"));
                    if (result != null)
                    {
                        if (result.HasElements)
                            return result.ToString();
                        return result.Value;
                    }
                }

                return soapResponse;
            }
            catch (Exception ex)
            {
                LogDebug($"Error en ExtractSoapResponse: {ex.Message}");
                return soapResponse;
            }
        }

        private List<CuentaClienteDTO> ParseCuentaClienteDTOList(string response)
        {
            var cuentas = new List<CuentaClienteDTO>();
            try
            {
                if (string.IsNullOrWhiteSpace(response))
                    return cuentas;

                XDocument doc = XDocument.Parse(response);
                XNamespace ns = "http://tempuri.org/";
                var elements = doc.Descendants(ns + "CuentaClienteDTO").ToList();

                if (elements.Count == 0)
                    elements = doc.Descendants("CuentaClienteDTO").ToList();

                foreach (var elem in elements)
                {
                    var cuenta = new CuentaClienteDTO
                    {
                        NumeroCuenta = elem.Element(ns + "NumeroCuenta")?.Value ?? elem.Element("NumeroCuenta")?.Value ?? "",
                        Saldo = decimal.Parse(elem.Element(ns + "Saldo")?.Value ?? elem.Element("Saldo")?.Value ?? "0"),
                        IdCliente = int.Parse(elem.Element(ns + "IdCliente")?.Value ?? elem.Element("IdCliente")?.Value ?? "0"),
                        NombreCliente = elem.Element(ns + "NombreCliente")?.Value ?? elem.Element("NombreCliente")?.Value ?? "",
                        ApellidoCliente = elem.Element(ns + "ApellidoCliente")?.Value ?? elem.Element("ApellidoCliente")?.Value ?? "",
                        Disponibilidad = elem.Element(ns + "Disponibilidad")?.Value ?? elem.Element("Disponibilidad")?.Value ?? ""
                    };
                    cuentas.Add(cuenta);
                }
            }
            catch (Exception ex)
            {
                LogDebug("Error parsing cuentas: " + ex.Message);
            }
            return cuentas;
        }

        private List<Movimiento> ParseMovimientoList(string response)
        {
            var movimientos = new List<Movimiento>();
            try
            {
                if (string.IsNullOrWhiteSpace(response))
                    return movimientos;

                XDocument doc = XDocument.Parse(response);
                XNamespace ns = "http://tempuri.org/";
                var elements = doc.Descendants(ns + "Movimiento").ToList();

                if (elements.Count == 0)
                    elements = doc.Descendants("Movimiento").ToList();

                foreach (var elem in elements)
                {
                    var mov = new Movimiento
                    {
                        NumeroOperacion = elem.Element(ns + "NumeroOperacion")?.Value ?? elem.Element("NumeroOperacion")?.Value ?? "",
                        NumeroCuenta = elem.Element(ns + "NumeroCuenta")?.Value ?? elem.Element("NumeroCuenta")?.Value ?? "",
                        IdEmpleado = int.Parse(elem.Element(ns + "IdEmpleado")?.Value ?? elem.Element("IdEmpleado")?.Value ?? "0"),
                        NombreEmpleado = elem.Element(ns + "NombreEmpleado")?.Value ?? elem.Element("NombreEmpleado")?.Value ?? "Desconocido",
                        Tipo = elem.Element(ns + "Tipo")?.Value ?? elem.Element("Tipo")?.Value ?? "",
                        Monto = decimal.Parse(elem.Element(ns + "Monto")?.Value ?? elem.Element("Monto")?.Value ?? "0"),
                        FechaHora = elem.Element(ns + "FechaHora")?.Value ?? elem.Element("FechaHora")?.Value ?? "N/A"
                    };
                    movimientos.Add(mov);
                }
            }
            catch (Exception ex)
            {
                LogDebug("Error parsing movimientos: " + ex.Message);
            }
            return movimientos;
        }

        private string XmlEscape(string text)
        {
            return System.Security.SecurityElement.Escape(text);
        }
    }
}
