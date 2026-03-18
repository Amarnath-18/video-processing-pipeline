using AuthService.Models.Auth;

namespace AuthService.DAL.AuthDal
{
    public interface IAuthDal
    {
        Task<(int Status, User? User, string? Message)> GetUserByEmail(string email);
        Task<(int Status, string? Message)> Register(Register register, string HashedPassword);
    }
}
