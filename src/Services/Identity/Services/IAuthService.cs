using NSE.Core.Messages.Integrations;
using NSE.Identity.API.Models;

namespace NSE.Identity.API.Services;

public interface IAuthService
{
    Task<UserLoginResponse> GenerateToken(string email);
    Task<ResponseMessage> RegisterCustomer(UserRegister userRegister);
}