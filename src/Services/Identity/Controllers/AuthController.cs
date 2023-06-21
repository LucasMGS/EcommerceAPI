using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSE.Identidade.API.Extensions;
using NSE.Identity.API.Models;
using NSE.Identity.API.Services;

namespace NSE.Identity.API.Controllers;

[Route("api/identity")]
public class AuthController : MainController
{
private readonly SignInManager<IdentityUser> _signInManager;
private readonly UserManager<IdentityUser> _userManager;
private readonly IAuthService _authService;
  public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IAuthService authService) 
  {
    _userManager = userManager;
    _signInManager = signInManager;
    _authService = authService;
  }

  [HttpPost("register")]
  public async Task<IActionResult> RegisterUser([FromBody] UserRegister userRegister)
  {
    if (!ModelState.IsValid) return CustomResponse(ModelState);
    var user = new IdentityUser
    {
      UserName = userRegister.Email,
      Email = userRegister.Email,
      EmailConfirmed = true,
    };

    var userResult = await _userManager.CreateAsync(user, userRegister!.Password!);
    if (userResult.Succeeded)
    {
      return CustomResponse(await _authService.GenerateToken(userRegister.Email!));
    }

    foreach (var error in userResult.Errors)
    {
      AddErrorToStack(error.Description);
    }

    return CustomResponse();
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] UserLogin user)
  {
    if (!ModelState.IsValid) return CustomResponse();
    var result = await _signInManager.PasswordSignInAsync(user.Email!, user.Password!, isPersistent: true, lockoutOnFailure: true);

    if (result.Succeeded)
    {
      return CustomResponse(await _authService.GenerateToken(user.Email!));
    }
    
    if(result.IsLockedOut) {
      AddErrorToStack("Usuário temporariamente bloqueado por tentativas inválidas");
      return CustomResponse();
    }

    AddErrorToStack("Usuário ou senha incorretos");
    return CustomResponse();
  }


 
}
