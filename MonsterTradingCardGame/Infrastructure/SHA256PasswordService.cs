using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MonsterTradingCardGame.Infrastructure
{
    public static class SHA256PasswordService
    {
        public static byte[] GenerateHash(byte[] password, byte[] salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                List<byte> saltedPw = new List<byte>();
                saltedPw.AddRange(password);
                saltedPw.AddRange(salt);

                return sha256.ComputeHash(saltedPw.ToArray());
            }
        }
    }
}
