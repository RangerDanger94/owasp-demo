using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OWASP.DAL;
using OWASP.DAL.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace OWASP.Auth
{
    public class CustomAuthHandler : AuthenticationHandler<CustomAuthOptions>
    {
        SecurityService _security;
        ApplicationDbContext _context;

        public CustomAuthHandler(IOptionsMonitor<CustomAuthOptions> options, 
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IServiceProvider provider) : base(options, logger, encoder, clock)
        {
            _context = ActivatorUtilities.CreateInstance<ApplicationDbContext>(provider);
            _security = ActivatorUtilities.CreateInstance<SecurityService>(provider);
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var username = Context.Request.Headers["X-Auth-Username"].FirstOrDefault();
            var password = Context.Request.Headers["X-Auth-Password"].FirstOrDefault();

            if (string.IsNullOrEmpty(username))
                return AuthenticateResult.NoResult();

            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Username == username);

            if(user == null)
                return AuthenticateResult.Fail("No user with that username exists");

            if (!_security.VerifyPassword(user, password))
                return AuthenticateResult.Fail("Incorrect password");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.EmailAddress),
            };

            var claimsIdentity = new ClaimsIdentity(claims, Authentication.DefaultAuthenticationScheme);
            var authenticationTicket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Authentication.DefaultAuthenticationScheme);
            return AuthenticateResult.Success(authenticationTicket);            
        }
    }
}
