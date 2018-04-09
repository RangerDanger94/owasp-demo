using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OWASP.DAL.Security
{
    public class SecurityService
    {
        private ISecurityOptions _securityOptions;
        public SecurityService(ISecurityOptions securityOptions)
        {
            _securityOptions = securityOptions;
        }

        // return [salt] + HMAC-SHA-256([key], [salt] + [credential]); 
        public string ProtectPassword(User user)
        {
            using (var algorithm = new HMACSHA256())
            {
                algorithm.Key = Encoding.UTF8.GetBytes(_securityOptions.Key);

                var saltBytes = Encoding.UTF8.GetBytes(user.Salt).ToList();
                var inputBytes = Encoding.UTF8.GetBytes(user.Password).ToList();
                byte[] toHash = saltBytes.Concat(inputBytes).ToArray();

                var hash = algorithm.ComputeHash(toHash);
                return Convert.ToBase64String(hash);
            }
        }

        public bool VerifyPassword(User user, string input)
        {
            using (var algorithm = new HMACSHA256())
            {
                algorithm.Key = Encoding.UTF8.GetBytes(_securityOptions.Key);

                var saltBytes = Encoding.UTF8.GetBytes(user.Salt).ToList();
                var inputBytes = Encoding.UTF8.GetBytes(input).ToList();
                byte[] toHash = saltBytes.Concat(inputBytes).ToArray();

                var hash = algorithm.ComputeHash(toHash);
                return Convert.ToBase64String(hash) == user.Password;
            }
        }

        public string GenerateSalt()
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
