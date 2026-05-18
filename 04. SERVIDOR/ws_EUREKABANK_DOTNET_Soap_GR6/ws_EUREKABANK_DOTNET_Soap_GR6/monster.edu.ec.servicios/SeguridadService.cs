using monster.edu.ec.dao;
using monster.edu.ec.dao.impl;
using monster.edu.ec.model;

namespace monster.edu.ec.servicios
{
    public class SeguridadService
    {
        private readonly IEmpleadoDAO _empleadoDAO;
        private readonly IClienteDAO _clienteDAO;

        public SeguridadService()
        {
            _empleadoDAO = new EmpleadoDAOImpl();
            _clienteDAO = new ClienteDAOImpl();
        }

        public Empleado Login(string usuario, string password)
        {
            return _empleadoDAO.ValidarLogin(usuario, password);
        }

        public Cliente LoginCliente(string dni)
        {
            return _clienteDAO.ValidarLogin(dni);
        }
    }
}
