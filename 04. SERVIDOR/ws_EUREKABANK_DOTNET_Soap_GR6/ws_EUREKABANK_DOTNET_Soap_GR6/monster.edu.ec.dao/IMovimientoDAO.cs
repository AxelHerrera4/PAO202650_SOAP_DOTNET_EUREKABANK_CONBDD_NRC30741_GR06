using monster.edu.ec.model;
using Microsoft.Data.SqlClient;

namespace monster.edu.ec.dao
{
    public interface IMovimientoDAO
    {
        bool RegistrarMovimiento(Movimiento movimiento);
        bool RegistrarMovimiento(SqlConnection con, Movimiento movimiento);
        bool RegistrarMovimiento(SqlConnection con, Movimiento movimiento, SqlTransaction transaction);
        List<Movimiento> ListarExtracto(string numeroCuenta);
    }
}
