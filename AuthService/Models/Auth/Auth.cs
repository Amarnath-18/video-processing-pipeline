using Common;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Auth
{
    public record Register
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "UserName must be between 3 and 50 characters")]
        public required string UserName { get; init; }
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; init; }
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 128 characters")]
        public required string Password { get; init; }
    }

    public record Login
    {
        [EmailAddress]
        public required string Email { get; init; }
        public required string Password { get; init; }
    }

    public record LoginResponse : CommonResponse
    {
        public string? AccessToken { get; init; }
    }

    public record UserDetails
    {
        public Guid Id { get; init; }
        public string UserName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public bool IsActive { get; init; }
    }

    //public record GetUserResponse : CommonResponse
    //{
    //    public UserDetails? User { get; init; }
    //}


    public record User
    {
        public Guid Id {  get; init; }
        public string UserName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string HashedPassword { get; init; } = string.Empty;
        public bool isActive { get; init; }
    }

    

}
