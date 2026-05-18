# 🏦 EurekaBankWS - Web Service SOAP

## 📋 Estructura MVC del Proyecto

Este proyecto implementa un **Web Service SOAP** siguiendo el patrón **MVC (Model-View-Controller)** con la arquitectura por capas.

```
ws_EUREKABANK_DOTNET_Soap_GR6/
│
├── 📁 monster.edu.ec.model/
│   ├── Empleado.cs
│   ├── Cliente.cs
│   ├── Cuenta.cs
│   ├── Movimiento.cs
│   └── CuentaClienteDTO.cs
│
├── 📁 monster.edu.ec.controlador/
│   └── EurekaBankWSControlador.cs (Web Service SOAP)
│
├── 📁 monster.edu.ec.vista/
│   ├── RespuestasVista.cs
│   └── (Clases de respuesta para presentación)
│
├── 📁 monster.edu.ec.servicios/
│   ├── SeguridadService.cs
│   ├── TransaccionService.cs
│   └── GestorConcurrencia.cs
│
├── 📁 monster.edu.ec.dao/
│   ├── IEmpleadoDAO.cs
│   ├── IClienteDAO.cs
│   ├── ICuentaDAO.cs
│   └── IMovimientoDAO.cs
│
├── 📁 monster.edu.ec.dao.impl/
│   ├── EmpleadoDAOImpl.cs
│   ├── ClienteDAOImpl.cs
│   ├── CuentaDAOImpl.cs
│   └── MovimientoDAOImpl.cs
│
├── 📁 monster.edu.ec.util/
│   ├── ConexionDB.cs
│   └── SeguridadUtil.cs
│
├── Program.cs
├── appsettings.json
└── ws_EUREKABANK_DOTNET_Soap_GR6.csproj
```

## 🎯 Descripción de Capas

### 🗂️ **Model (monster.edu.ec.model)**
Contiene las clases de modelo que representan las entidades del negocio:
- **Empleado**: Datos de empleados del banco
- **Cliente**: Datos de clientes
- **Cuenta**: Información de cuentas bancarias
- **Movimiento**: Transacciones realizadas
- **CuentaClienteDTO**: Objeto de transferencia de datos

### 🎮 **Controlador (monster.edu.ec.controlador)**
- **EurekaBankWSControlador**: Web Service SOAP que expone los servicios
  - Recibe las solicitudes del cliente
  - Orquesta las llamadas a servicios
  - Retorna las respuestas

### 👁️ **Vista (monster.edu.ec.vista)**
Clases de respuesta para la presentación de datos:
- **RespuestaAutenticacion**: Respuesta de login
- **RespuestaTransaccion**: Respuesta de operaciones
- **RespuestaConsulta<T>**: Respuesta de consultas
- **RespuestaError**: Respuesta de errores

### 🔧 **Servicios (monster.edu.ec.servicios)**
Lógica de negocio:
- **SeguridadService**: Autenticación de usuarios
- **TransaccionService**: Gestión de operaciones bancarias
- **GestorConcurrencia**: Control de acceso concurrente (Patrón Singleton)

### 💾 **DAO (monster.edu.ec.dao + monster.edu.ec.dao.impl)**
Acceso a datos:
- Interfaces en `monster.edu.ec.dao/`
- Implementaciones en `monster.edu.ec.dao.impl/`
- Consultas a la base de datos SQL Server

### 🛠️ **Utilitarios (monster.edu.ec.util)**
- **ConexionDB**: Gestión de conexiones
- **SeguridadUtil**: Funciones criptográficas

## 📡 Operaciones SOAP Disponibles

```xml
<!-- Autenticación -->
<AutenticarUsuario usuario="string" clave="string" />

<!-- Transacciones -->
<Depositar cuentaId="string" monto="double" empleadoId="int" />
<Retirar cuentaId="string" monto="double" empleadoId="int" />
<Transferir cuentaOrigenId="string" cuentaDestinoId="string" monto="double" empleadoId="int" />

<!-- Consultas -->
<ListarCuentasPorSucursal sucursalId="int" />
<ConsultarExtracto cuentaId="string" />
<ConsultarCuentasPorCliente dni="string" />
```

## 🔐 Características de Seguridad

✅ **Contraseñas Hasheadas**: Usando BCrypt.Net-Next  
✅ **Control de Concurrencia**: Bloqueos UPDLOCK, ROWLOCK  
✅ **Transacciones ACID**: Commit/Rollback automático  
✅ **Parametrización SQL**: Prevención de SQL Injection  

## 📦 Dependencias

```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.2.0" />
<PackageReference Include="Microsoft.Data.SqlClient" Version="7.0.1" />
<PackageReference Include="SoapCore" Version="1.2.1.13" />
```

## 🚀 Cómo Usar

### 1. Configurar Conexión a BD
Editar `appsettings.json`:
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EurekaBankDB;Integrated Security=True;TrustServerCertificate=True;"
}
```

### 2. Ejecutar la Aplicación
```powershell
dotnet run
```

### 3. Acceder al WSDL
```
http://localhost:5000/EurekaBankWS.asmx?wsdl
```

## 📚 Patrón de Diseño

| Patrón | Ubicación |
|--------|-----------|
| **MVC** | Estructura general |
| **DAO** | monster.edu.ec.dao* |
| **Singleton** | GestorConcurrencia |
| **DTO** | CuentaClienteDTO |
| **Service Locator** | SeguridadService, TransaccionService |

## 🔄 Flujo de una Transacción

```
Cliente (SOAP)
    ↓
EurekaBankWSControlador (Controlador)
    ↓
TransaccionService (Negocio)
    ↓
GestorConcurrencia (Bloqueo)
    ↓
CuentaDAOImpl + MovimientoDAOImpl (Datos)
    ↓
SqlConnection (BD)
```

## ✅ Estado del Proyecto

- ✅ Estructura MVC implementada
- ✅ Web Service SOAP configurado
- ✅ Seguridad con BCrypt
- ✅ Control de concurrencia
- ✅ Transacciones ACID
- ✅ Compilación exitosa
- ✅ .NET 10

---

**Desarrollado por**: PAO202650_RESTFUL_DOTNET_SINBDD_NRC30741_GR06  
**Versión**: 1.0  
**Fecha**: 2024
