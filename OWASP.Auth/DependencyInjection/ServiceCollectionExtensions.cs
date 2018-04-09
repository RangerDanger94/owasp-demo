using Microsoft.AspNetCore.Authentication;
using System;

namespace OWASP.Auth.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static AuthenticationBuilder AddCustomAuth(this AuthenticationBuilder builder)
        {
            return builder.AddScheme<CustomAuthOptions, CustomAuthHandler>(Authentication.DefaultAuthenticationScheme, 
                Authentication.DefaultAuthenticationScheme, options => { });
        }

        public static AuthenticationBuilder AddCustomAuth(this AuthenticationBuilder builder, Action<CustomAuthOptions> configureOptions)
        {
            return builder.AddScheme<CustomAuthOptions, CustomAuthHandler>(Authentication.DefaultAuthenticationScheme, 
                Authentication.DefaultAuthenticationScheme, configureOptions);
        }
    }
}
