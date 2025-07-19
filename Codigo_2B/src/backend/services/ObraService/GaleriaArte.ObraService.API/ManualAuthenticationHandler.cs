using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

public class ManualAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfiguration _configuration;

    public ManualAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration configuration)
        : base(options, logger, encoder, clock)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var validator = new JwtManualValidator(
            secret: _configuration["Jwt:Secret"]!,
            issuer: _configuration["Jwt:Issuer"]!,
            audience: _configuration["Jwt:Audience"]!
        );

        if (!validator.ValidarToken(token, out var claimsDict, out var error))
        {
            Logger.LogWarning("[ManualAuth] Token inválido: {0}", error);
            return Task.FromResult(AuthenticateResult.Fail("Token inválido: " + error));
        }

        // Convertir claims del diccionario a lista de Claim
        var claims = claimsDict.Select(kv => new Claim(kv.Key, kv.Value.ToString()!)).ToList();

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
