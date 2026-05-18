namespace monster.edu.ec.model
{
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
}
