namespace monster.edu.ec.model
{
    public class Cuenta
    {
        public string NumeroCuenta { get; set; }
        public decimal Saldo { get; set; }
        public int IdCliente { get; set; }
        public int IdSucursal { get; set; }
        public string Estado { get; set; }
    }
}
