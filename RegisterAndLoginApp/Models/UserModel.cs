
using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace RegisterAndLoginApp.Models
{

    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;


        public string PasswordHash { get; set; } = string.Empty;
        public byte[] Salt { get; set; } = Array.Empty<byte>();

        public string Groups { get; set; } = string.Empty;

        public void SetPassword(string password)
        {
            Salt = RandomNumberGenerator.GetBytes(16);

     
            var derived = KeyDerivation.Pbkdf2(
                password: password,
                salt: Salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 600000, 
                numBytesRequested: 32);

            PasswordHash = Convert.ToBase64String(derived);
        }

        public bool VerifyPassword(string password)
        {
            if (Salt == null || Salt.Length == 0 || string.IsNullOrEmpty(PasswordHash))
                return false;

            var derived = KeyDerivation.Pbkdf2(
                password: password,
                salt: Salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 600000,
                numBytesRequested: 32);

            var candidate = Convert.ToBase64String(derived);
            return string.Equals(candidate, PasswordHash, StringComparison.Ordinal);
        }
    }
}
