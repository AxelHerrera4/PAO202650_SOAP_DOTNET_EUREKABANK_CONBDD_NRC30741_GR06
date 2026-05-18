using monster.edu.ec.model;

namespace monster.edu.ec.dao
{
    public interface IClienteDAO
    {
        Cliente ValidarLogin(string dni);
    }
}
