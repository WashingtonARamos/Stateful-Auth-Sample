using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stateful_Auth_Sample.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static Stateful_Auth_Sample.Models.User;

namespace Stateful_Auth_Sample.Authentication
{
    public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private DataContext _data;

        public CustomAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, DataContext data)
            : base(options, logger, encoder, clock) => _data = data;

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // The token should be in a request header. Tipically you'd place it in a "Bearer" header, if it isn't
            // present return a NoResult right away.
            if (!Request.Headers.ContainsKey("Bearer"))
                return await Task.FromResult(AuthenticateResult.NoResult());

            var bearer = Request.Headers["Bearer"].ToString();

            // After obtaining the token you should make the correct validations to make sure it's a valid token,
            // in this case I'll make sure it isn't null or empty & check if it's exactly 8 characters long, which
            // is the length used when generating the token when a user logs in.

            if (string.IsNullOrEmpty(bearer) || bearer.Length != 8)
                return await Task.FromResult(AuthenticateResult.NoResult());

            // If the token is valid, search for the user in the database. If user is not present, NoResult().

            var user = _data.Users.Select(s => new { s.AccessToken, s.Id, s.Role }).FirstOrDefault(s => s.AccessToken == bearer);
            if (user == null)
                return await Task.FromResult(AuthenticateResult.NoResult());

            // If user is present, build it's AuthenticationTicket.

            var claims = new List<Claim>();

            claims.Add(new Claim("Id", user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Role, user.Role switch
            {
                UserRole.Reader => Roles.Reader,
                UserRole.Creator => Roles.Creator,
                _ => null
            }));

            var identity = new ClaimsIdentity(claims, CustomAuthenticationExtensions.CustomScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, CustomAuthenticationExtensions.CustomScheme);

            return await Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
