using AuthService.DAL.AuthDal;
using AuthService.Models.Auth;
using AuthService.Models.Common;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> logger;
        private readonly IAuthDal authDal;
        private readonly IConfiguration _config;
        public AuthService(ILogger<AuthService> logger, IAuthDal authDal, IConfiguration config)
        {
            this.logger = logger;
            this.authDal = authDal;
            _config = config;
        }

        public async Task<(int Status, string? Message)> Register(Register register)
        {
            try
            {
                if (register == null)
                {
                    logger.LogWarning("register request is null");
                    return (AppStatusCode.INVALID_INPUT, "Invalid Request!!");
                }

                if (register.UserName == null)
                {
                    logger.LogWarning("UserName is null");
                    return (AppStatusCode.INVALID_INPUT, "UserName is Required!!!");
                }
                if (register.Password == null)
                {
                    logger.LogWarning("Password is null");
                    return (AppStatusCode.INVALID_INPUT, "Password is Required!!");
                }
                if (register.Email == null)
                {
                    logger.LogWarning("Email is null");
                    return (AppStatusCode.INVALID_INPUT, "Email is Required!!");
                }

                var userResponse = await authDal.GetUserByEmail(register.Email);
                if (userResponse.Status == AppStatusCode.DATABASE_ERROR)
                {
                    logger.LogError("Failed to validate the user, Please try again!!");
                    return (AppStatusCode.DATABASE_ERROR, "Failed to validate the user, Please try again!!");
                }
                if (userResponse.User != null)
                {
                    logger.LogWarning("User is already Exsits");
                    return (AppStatusCode.DUPLICATE_RECORD, "User is already Exsits!!");
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(register.Password);

                var registerUserResponse = await authDal.Register(register, hashedPassword);
                if (registerUserResponse.Status != AppStatusCode.SUCCESS)
                {
                    return (registerUserResponse.Status, registerUserResponse.Message);
                }
                return (registerUserResponse.Status, registerUserResponse.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to Create User for User {userName} ", register.UserName);
                return (AppStatusCode.INTERNAL_SERVER_ERROR, "Failed to Create User");
            }
        }

        public async Task<(int Status, string? AccessToken, string? Message)> Login(Login login)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(login.Email))
                {
                    logger.LogWarning("Email can not null or whitespace");
                    return (AppStatusCode.INVALID_CREDENTIALS, null, "Email can not empty or whitespace!!");
                }
                if (string.IsNullOrWhiteSpace(login.Password))
                {
                    logger.LogWarning($"Password is invalid: {login.Password}");
                    return (AppStatusCode.INVALID_CREDENTIALS, null, "Password is required!!");
                }

                var userResponse = await authDal.GetUserByEmail(login.Email);
                if (userResponse.Status == AppStatusCode.DATABASE_ERROR)
                {
                    return (userResponse.Status, null, "Failed to validate User Please try again!");
                }
                if (userResponse.User == null)
                {
                    return (AppStatusCode.BAD_REQUEST, null, "Invalid Email Or Password!!");
                }

                if (!BCrypt.Net.BCrypt.Verify(login.Password, userResponse.User.HashedPassword))
                {
                    logger.LogWarning($"Invalid Password for ${login.Email}");
                    return (AppStatusCode.BAD_REQUEST, null, "Invalid Email Or Password!!");
                }

                string token = GenerateToken(userResponse.User);
                return (AppStatusCode.SUCCESS, token, "Login Successfull");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to login user for {Email}", login.Email);
                return (AppStatusCode.BAD_REQUEST, null, ex.Message);
            }
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "")
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("userName", user.UserName) // optional custom claims
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_config["Jwt:ExpiryMinutes"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
