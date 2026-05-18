using Microsoft.Data.SqlClient;

namespace monster.edu.ec.util
{
    public static class ConexionDB
    {
        private static readonly string ConnectionString =
            "Server=localhost;Database=EurekaBankDB;User Id=sa;Password=12345678;TrustServerCertificate=True;";

        public static SqlConnection GetConexion()
        {
            try
            {
                LogMessage($"[DB] Conectando a: {ConnectionString}");
                var connection = new SqlConnection(ConnectionString);
                connection.Open();
                LogMessage("[DB] ✓ Conexión exitosa");
                return connection;
            }
            catch (Exception ex)
            {
                LogMessage($"[DB] ✗ Error: {ex.Message}");
                LogMessage($"[DB] Inner: {ex.InnerException?.Message}");
                return null;
            }
        }

        private static void LogMessage(string message)
        {
            try
            {
                string logPath = "C:\\Logs\\EurekaBankDB.log";
                string logDir = Path.GetDirectoryName(logPath);
                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);
                File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
            }
            catch { }
        }
    }
}
