using System.Security.Cryptography;
using System.Text;

namespace API.Common.Utils
{
    public static class Password
    {
        public static string Hash(string password)
        {
            return Encoding.UTF8.GetString(SHA256.HashData(Encoding.ASCII.GetBytes(password)));
        }

        public static bool ValidatePassword(string hashPass, string password)
        {
            var check = Hash(password);
            return check == hashPass;
        }
    }
}
