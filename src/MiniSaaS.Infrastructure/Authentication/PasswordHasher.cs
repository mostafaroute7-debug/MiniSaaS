using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MiniSaaS.Application.Common.Interfaces;
using System.Security.Cryptography;

namespace MiniSaaS.Infrastructure.Authentication;

sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);

        var hash = KeyDerivation.Pbkdf2(password, salt,KeyDerivationPrf.HMACSHA256,100_000,32);

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password,string passwordHash)
    {
        var parts = passwordHash.Split('.');

        if (parts.Length != 2)
            return false;

        var salt = Convert.FromBase64String(parts[0]);
        var storedHash = Convert.FromBase64String(parts[1]);

        var hash = KeyDerivation.Pbkdf2(password,salt,KeyDerivationPrf.HMACSHA256,100_000,32);

        return CryptographicOperations.FixedTimeEquals(hash,storedHash);
    }
}