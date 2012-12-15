using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace QLogBrowser.Helpers
{
    public class SecurityHelper
    {
        private static byte[] salt = Encoding.UTF8.GetBytes(Properties.Settings.Default.SecuritySalt);

        public static string Decrypt(string input)
        {
            try
            {
                byte[] decryptedBytes = ProtectedData.Unprotect(Convert.FromBase64String(input), salt, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch
            {
                return "";
            }
        }

        public static string Encrypt(string input)
        {
            byte[] encryptedBytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(input), salt, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedBytes);
        }
    }
}
