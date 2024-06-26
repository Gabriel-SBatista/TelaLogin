using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Login.Core.Presenter;

namespace Login.Core.Services.Hasher
{
    public class PasswordHasher : IPasswordHasher
    {
        public Password HashPassowrd(string password)
        {
            byte[] saltBytes = new byte[16];
            RandomNumberGenerator.Fill(saltBytes);
            string salt = Convert.ToBase64String(saltBytes);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = saltBytes,
                DegreeOfParallelism = 8,
                MemorySize = 65536,
                Iterations = 4
            };

            byte[] hashBytes = argon2.GetBytes(32);
            string hash = Convert.ToBase64String(hashBytes);

            return new Password { Hash = hash, Salt = salt};
        }

        public bool VerifyPassword(string password, string hash, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = saltBytes,
                DegreeOfParallelism = 8,
                MemorySize = 65536,
                Iterations = 4
            };

            byte[] hashBytes = argon2.GetBytes(32);
            string computedHash = Convert.ToBase64String(hashBytes);

            return computedHash == hash;
        }
    }
}
