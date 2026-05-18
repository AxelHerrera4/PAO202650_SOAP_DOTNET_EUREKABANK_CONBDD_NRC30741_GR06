using monster.edu.ec.model;
using Microsoft.Data.SqlClient;

namespace monster.edu.ec.dao
{
    public interface ICuentaDAO
    {
        Cuenta ObtenerCuenta(string numeroCuenta);
        Cuenta ObtenerCuentaForUpdate(SqlConnection con, string numeroCuenta);
        Cuenta ObtenerCuentaForUpdate(SqlConnection con, string numeroCuenta, SqlTransaction transaction);
        bool ActualizarSaldo(SqlConnection con, string numeroCuenta, decimal nuevoSaldo);
        bool ActualizarSaldo(SqlConnection con, string numeroCuenta, decimal nuevoSaldo, SqlTransaction transaction);
        bool ActualizarSaldo(string numeroCuenta, decimal nuevoSaldo);
        List<CuentaClienteDTO> ObtenerCuentasConClientes(int sucursalId);
        CuentaClienteDTO ObtenerCuentaPorDni(string dni);
    }
}
