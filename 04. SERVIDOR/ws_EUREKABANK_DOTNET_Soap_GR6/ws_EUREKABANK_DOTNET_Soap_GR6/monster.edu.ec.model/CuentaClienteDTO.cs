namespace monster.edu.ec.model
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
}
