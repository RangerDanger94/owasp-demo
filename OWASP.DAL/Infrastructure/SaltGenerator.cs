using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Security.Cryptography;

namespace OWASP.DAL.Infrastructure
{
    public class SaltGenerator : ValueGenerator<string>
    {
        public override bool GeneratesTemporaryValues => false;

        public override string Next(EntityEntry entry)
        {
            using (var random = RNGCryptoServiceProvider.Create())
            {
                byte[] secureSalt = new byte[64];
                random.GetBytes(secureSalt);
                return Convert.ToBase64String(secureSalt);
            }
        }
    }
}
