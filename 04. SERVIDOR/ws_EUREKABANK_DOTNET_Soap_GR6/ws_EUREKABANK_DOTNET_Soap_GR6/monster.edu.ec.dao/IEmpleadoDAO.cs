using monster.edu.ec.model;

namespace monster.edu.ec.dao
{
    public interface IEmpleadoDAO
    {
        Empleado ValidarLogin(string usuario, string password);
    }
}
