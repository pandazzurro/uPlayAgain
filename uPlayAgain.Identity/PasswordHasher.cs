using Microsoft.AspNet.Identity;
using System;
using System.Security.Cryptography;

namespace uPlayAgain.Utilities
{
    public class PasswordHasher : IPasswordHasher
    {
        private NLog.Logger _log = NLog.LogManager.GetLogger("uPlayAgain");
        public string HashPassword(string password)
        {
            string result = string.Empty;
            try
            {
                byte[] salt;
                byte[] buffer2;
                if (password == null)
                {
                    throw new ArgumentNullException("password");
                }
                using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
                {
                    salt = bytes.Salt;
                    buffer2 = bytes.GetBytes(0x20);
                }
                byte[] dst = new byte[0x31];
                Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
                Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
                return Convert.ToBase64String(dst);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return result;
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            PasswordVerificationResult response = PasswordVerificationResult.Failed;
            try
            {
                byte[] buffer4;
                if (hashedPassword == null)
                {
                    return PasswordVerificationResult.Failed;
                }
                if (providedPassword == null)
                {
                    throw new ArgumentNullException("password");
                }
                byte[] src = Convert.FromBase64String(hashedPassword);
                if ((src.Length != 0x31) || (src[0] != 0))
                {
                    return PasswordVerificationResult.Failed;
                }
                byte[] dst = new byte[0x10];
                Buffer.BlockCopy(src, 1, dst, 0, 0x10);
                byte[] buffer3 = new byte[0x20];
                Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
                using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(providedPassword, dst, 0x3e8))
                {
                    buffer4 = bytes.GetBytes(0x20);
                }
                bool result = ByteArraysEqual(buffer3, buffer4);
                if (result)
                    return PasswordVerificationResult.Success;
                else
                    return PasswordVerificationResult.Failed;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return response;
        }

        /// <summary>Compares two byte arrays for equality.</summary>
        /// <param name="b0">First byte array.</param><param name="b1">Second byte array.</param>
        /// <returns>true if the arrays are equal; false otherwise.</returns>
        private static bool ByteArraysEqual(byte[] b0, byte[] b1)
        {
            if (b0 == b1)
            {
                return true;
            }

            if (b0 == null || b1 == null)
            {
                return false;
            }

            if (b0.Length != b1.Length)
            {
                return false;
            }

            for (int i = 0; i < b0.Length; i++)
            {
                if (b0[i] != b1[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}