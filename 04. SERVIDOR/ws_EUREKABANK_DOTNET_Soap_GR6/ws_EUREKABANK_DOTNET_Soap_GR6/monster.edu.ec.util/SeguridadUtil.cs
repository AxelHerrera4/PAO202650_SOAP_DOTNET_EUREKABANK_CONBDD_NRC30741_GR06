using BCrypt.Net;

namespace monster.edu.ec.util
{
    public static class SeguridadUtil
    {
        public static bool CheckPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                return false;
            }
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 10);
        }
    }
}
