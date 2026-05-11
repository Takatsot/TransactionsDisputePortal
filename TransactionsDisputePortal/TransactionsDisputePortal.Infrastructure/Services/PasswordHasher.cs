using System;
using System.Security.Cryptography;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using TransactionsDisputePortal.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Infrastructure.Services
{
    /// <summary>
    /// Service for hashing and verifying passwords using PBKDF2
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128 bits
        private const int HashSize = 32; // 256 bits
        private const int Iterations = 100000;

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            // Generate salt
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash password with salt
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: Iterations,
                numBytesRequested: HashSize);

            // Combine salt and hash
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convert to base64
            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(hash))
                return false;

            try
            {
                // Convert base64 to bytes
                byte[] hashBytes = Convert.FromBase64String(hash);

                // Extract salt
                byte[] salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                // Hash the input password with the same salt
                byte[] computedHash = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: Iterations,
                    numBytesRequested: HashSize);

                // Compare hashes
                for (int i = 0; i < HashSize; i++)
                {
                    if (hashBytes[i + SaltSize] != computedHash[i])
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
