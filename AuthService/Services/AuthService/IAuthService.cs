using AuthService.Models.Auth;

namespace AuthService.Services.AuthService
{
    public interface IAuthService
    {
        Task<(int Status, string? Message)> Register(Register register);
        Task<(int Status, string? AccessToken, string? Message)> Login(Login login);
    }
}
