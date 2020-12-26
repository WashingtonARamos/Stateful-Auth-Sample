using Microsoft.AspNetCore.Authentication;
using System;

namespace Stateful_Auth_Sample.Authentication
{
    public static class CustomAuthenticationExtensions
    {

        public const string CustomScheme = nameof(CustomScheme);
        public const string CustomAuth = nameof(CustomAuth);

        public static AuthenticationBuilder AddCustomAuthentication(this AuthenticationBuilder builder, Action<AuthenticationSchemeOptions> configureOptions)
            => builder.AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>(CustomScheme, CustomAuth, configureOptions);

    }
}
