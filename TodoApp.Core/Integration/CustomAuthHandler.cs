using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
namespace TodoApp.Core.Integration
{
    public class CustomAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public CustomAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, "test-user"),
        new Claim(ClaimTypes.Role, "Owner")  
        };
            var identity = new ClaimsIdentity(claims, "Test", ClaimTypes.NameIdentifier, ClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

    }
}