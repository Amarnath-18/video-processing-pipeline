using AuthService.Models.Auth;
using AuthService.Models.Common;
using Dapper;
using System.Data;

namespace AuthService.DAL.AuthDal
{
    public class AuthDal : IAuthDal
    {
        private readonly ILogger<AuthDal> logger;
        private readonly IDbConnection dbConnection;

        public AuthDal(ILogger<AuthDal> logger , IDbConnection dbConnection)
        {
            this.logger = logger;
            this.dbConnection = dbConnection;
        }

        public async Task<(int Status, string? Message)> Register(Register register, string HashedPassword)
        {
            try
            {
                const string sql = @"INSERT INTO users (id, username, email, password_hash, is_active, created_at, updated_at ) VALUES (@Id, @UserName, @Email, @Password_Hash, @ActiveStatus, @CreatedAt, @UpdatedAt)";
                var parameters = new
                {
                    Id = Guid.NewGuid(),
                    UserName = register.UserName,
                    Email = register.Email.ToLower(),
                    Password_Hash = HashedPassword,
                    ActiveStatus = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await dbConnection.ExecuteAsync(sql, parameters);
                return (AppStatusCode.SUCCESS, "User Created Successfull");

            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error while registering user {Email}", register.Email);
                return (AppStatusCode.DATABASE_ERROR, "User creation failed");
            }
        }

        public async Task<(int Status, User? User, string? Message)> GetUserByEmail(string email)
        {
            try
            {
                const string sql = @"
            SELECT 
                id as Id, 
                username as UserName, 
                email as Email, 
                password_hash as HashedPassword, 
                is_active as IsActive 
            FROM users 
            WHERE email = @Email";

                var user = await dbConnection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });

                if (user == null)
                {
                    return (AppStatusCode.RECORD_NOT_FOUND, null, "User not found");
                }

                return (AppStatusCode.SUCCESS, user, null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching user with email: {Email}", email);
                return (AppStatusCode.DATABASE_ERROR, null, "Something went wrong");
            }
        }
    }
}
