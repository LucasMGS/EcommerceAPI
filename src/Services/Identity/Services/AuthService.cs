using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSE.Core.Mediator;
using NSE.Core.Messages.Integration;
using NSE.Core.Messages.Integrations;
using NSE.Core.Messaging;
using NSE.Identity.API.Models;
using NSE.WebAPI.Core.Identidade;

namespace NSE.Identity.API.Services;

public class AuthService : IAuthService
{

    private readonly UserManager<IdentityUser> _userManager;
    private readonly JWTSettings _jwtSettings;
    private readonly IServiceProvider _serviceProvider;

    public AuthService(
        UserManager<IdentityUser> userManager,
        IOptions<JWTSettings> jwtSettings,
        IServiceProvider serviceProvider)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
        _serviceProvider = serviceProvider;
    }

  public async Task<UserLoginResponse> GenerateToken(string email)
  {
    var user = await _userManager.FindByEmailAsync(email);
    var claims = await _userManager.GetClaimsAsync(user!);
    var identityClaims = await GetUserClaims(claims, user!);
    var encodedToken = CodifyToken(identityClaims);

    return GetUserTokenResponse(encodedToken, user!, claims);
  }

    private string CodifyToken(ClaimsIdentity claimsIdentity)
    {

        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
          Issuer = _jwtSettings.Issuer,
          Audience = _jwtSettings.Audience,
          Subject = claimsIdentity,
          Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
          SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var encodedToken = tokenHandler.WriteToken(token);
        return encodedToken;
    }

    private UserLoginResponse GetUserTokenResponse(string encodedToken, IdentityUser user, IEnumerable<Claim> claims)
    {
        return new UserLoginResponse
        {
            AccessToken = encodedToken,
            ExpiresIn = TimeSpan.FromHours(_jwtSettings.ExpirationHours).TotalSeconds,
            UsuarioToken = new UserToken
            {
                Id = user.Id,
                Email = user.Email!,
                Claims = claims.Select(c => new UserClaim { Type = c.Type, Value = c.Value })
            }
        };
    }

    private async Task<ClaimsIdentity> GetUserClaims(ICollection<Claim> claims, IdentityUser user)
    {

        var userRoles = await _userManager.GetRolesAsync(user!);

        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email!));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iss, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

        foreach (var userRole in userRoles)
        {
            claims.Add(new Claim("role", userRole));
        }

        var identityClaims = new ClaimsIdentity();
        identityClaims.AddClaims(claims);
        return identityClaims;
    }

    private static long ToUnixEpochDate(DateTime date) =>
        (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

    public async Task<ResponseMessage> RegisterCustomer(UserRegister userRegister)
    {
        var user = await _userManager.FindByEmailAsync(userRegister.Email!);

        var customer = new RegisteredUserIntegrationEvent(
            Guid.Parse(user!.Id),
            userRegister.Nome,
            userRegister.Email!,
            userRegister.Cpf);

        using var scope = _serviceProvider.CreateScope();
        var messageProducer = scope.ServiceProvider.GetRequiredService<IMessageProducer>();
        messageProducer.Publish(customer, QueueNames.RegisterCustomer);

        return new ResponseMessage(new FluentValidation.Results.ValidationResult());
    }
}