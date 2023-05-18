using Asadotela.Api.Models;

namespace Asadotela.Api.Services;

public interface IAuthManager
{
    Task<bool> ValidateUser(LoginDTO userDTO);
    Task<string> CreatrToken();
}