using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSE.Core.Messages.Integration;
using NSE.Core.Messages.Integrations;
using NSE.Identity.API.Models;
using NSE.Identity.API.Services;
using NSE.WebAPI.Core.Controllers;

namespace NSE.Identity.API.Controllers;

[Route("api/[controller]")]
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
            await _authService.RegisterCustomer(userRegister);
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
