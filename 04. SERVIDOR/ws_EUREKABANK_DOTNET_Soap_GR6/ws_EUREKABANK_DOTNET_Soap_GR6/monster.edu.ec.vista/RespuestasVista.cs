namespace monster.edu.ec.vista
{
    /// <summary>
    /// Clase de vista para la respuesta de autenticación
    /// </summary>
    public class RespuestaAutenticacion
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public object Datos { get; set; }
    }

    /// <summary>
    /// Clase de vista para la respuesta de transacciones
    /// </summary>
    public class RespuestaTransaccion
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public string NumeroOperacion { get; set; }
        public decimal SaldoActual { get; set; }
    }

    /// <summary>
    /// Clase de vista para la respuesta de consultas
    /// </summary>
    public class RespuestaConsulta<T>
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public List<T> Datos { get; set; }
    }

    /// <summary>
    /// Clase de vista para errores
    /// </summary>
    public class RespuestaError
    {
        public bool Exito { get; set; } = false;
        public string Mensaje { get; set; }
        public string Codigo { get; set; }
    }
}
