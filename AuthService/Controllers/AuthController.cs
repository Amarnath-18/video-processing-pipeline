using AuthService.Models.Auth;
using Common;
using AuthService.Services.AuthService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> logger;
        private readonly IAuthService authService;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            this.authService = authService;
            this.logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login login)
        {
            try
            {
                if (login == null)
                {
                    return BadRequest(new LoginResponse
                    {
                        Status = AppStatusCode.BAD_REQUEST,
                        AccessToken = null,
                        Message = "Invalid Request, Please try again!!"
                    });
                }
                var response = await authService.Login(login);

                LoginResponse loginResponse = new LoginResponse
                {
                    Status = response.Status,
                    AccessToken = response.AccessToken,
                    Message = response.Message
                };


                switch (loginResponse.Status)
                {
                    case AppStatusCode.SUCCESS:
                        return Ok(loginResponse);
                    case AppStatusCode.INVALID_CREDENTIALS:
                        return Unauthorized(loginResponse);
                    case AppStatusCode.RECORD_NOT_FOUND:
                        return NotFound(loginResponse);
                    case AppStatusCode.BAD_REQUEST:
                        return BadRequest(loginResponse);
                    default:
                        return StatusCode(500, loginResponse);
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during login");
                return StatusCode(500, new LoginResponse
                {
                    Status = AppStatusCode.INTERNAL_SERVER_ERROR,
                    AccessToken = null,
                    Message = "An error occurred during login"
                });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Register register)
        {
            try
            {
                // Validate request body
                if (register == null)
                {
                    logger.LogWarning("Register request is null");
                    return BadRequest(new CommonResponse
                    {
                        Status = AppStatusCode.BAD_REQUEST,
                        Message = "Invalid request body. Please provide required fields."
                    });
                }

                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault();

                    logger.LogWarning("Model validation failed: {errors}", errors);
                    return BadRequest(new CommonResponse
                    {
                        Status = AppStatusCode.VALIDATION_ERROR,
                        Message = errors ?? "Validation failed. Please check your input."
                    });
                }

                // Validate individual fields (additional safety checks)
                if (string.IsNullOrWhiteSpace(register.UserName))
                {
                    logger.LogWarning("UserName is null or empty");
                    return BadRequest(new CommonResponse
                    {
                        Status = AppStatusCode.REQUIRED_FIELD_MISSING,
                        Message = "UserName is required and cannot be empty."
                    });
                }

                if (string.IsNullOrWhiteSpace(register.Email))
                {
                    logger.LogWarning("Email is null or empty");
                    return BadRequest(new CommonResponse
                    {
                        Status = AppStatusCode.REQUIRED_FIELD_MISSING,
                        Message = "Email is required and cannot be empty."
                    });
                }

                if (string.IsNullOrWhiteSpace(register.Password))
                {
                    logger.LogWarning("Password is null or empty");
                    return BadRequest(new CommonResponse
                    {
                        Status = AppStatusCode.REQUIRED_FIELD_MISSING,
                        Message = "Password is required and cannot be empty."
                    });
                }

                // Call service to register user
                var (status, message) = await authService.Register(register);

                switch (status)
                {
                    case AppStatusCode.SUCCESS:
                        logger.LogInformation("User registered successfully: {email}", register.Email);
                        return Ok(new CommonResponse
                        {
                            Status = status,
                            Message = message ?? "User registered successfully. Please log in."
                        });

                    case AppStatusCode.DUPLICATE_RECORD:
                        logger.LogWarning("User already exists: {email}", register.Email);
                        return Conflict(new CommonResponse
                        {
                            Status = status,
                            Message = message ?? "Email is already registered. Please use a different email."
                        });

                    case AppStatusCode.VALIDATION_ERROR:
                    case AppStatusCode.INVALID_INPUT:
                    case AppStatusCode.REQUIRED_FIELD_MISSING:
                        logger.LogWarning("Validation error during registration: {message}", message);
                        return BadRequest(new CommonResponse
                        {
                            Status = status,
                            Message = message ?? "Validation failed. Please check your input."
                        });

                    case AppStatusCode.DATABASE_ERROR:
                        logger.LogError("Database error during registration: {message}", message);
                        return StatusCode(503, new CommonResponse
                        {
                            Status = status,
                            Message = "Service temporarily unavailable. Please try again later."
                        });

                    case AppStatusCode.INTERNAL_SERVER_ERROR:
                    default:
                        logger.LogError("Unexpected error during registration: {message}", message);
                        return StatusCode(500, new CommonResponse
                        {
                            Status = status,
                            Message = "An unexpected error occurred during registration. Please try again."
                        });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Critical error during user registration for email: {email}", register?.Email ?? "unknown");
                return StatusCode(500, new CommonResponse
                {
                    Status = AppStatusCode.INTERNAL_SERVER_ERROR,
                    Message = "An unexpected error occurred. Please contact support if the issue persists."
                });
            }
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                string? id  = User.FindFirst("UserId")?.Value;

                if (!Guid.TryParse(id, out Guid userId))
                {
                    return BadRequest(new CommonResponse
                    {
                        Status = AppStatusCode.FORBIDDEN,
                        Message = "Invalid UserId"
                    });
                }


                var response = await authService.GetUser(userId);
                var commonResponse = new CommonResponse
                {
                    Status = response.Status,
                    Message = response.Message,
                };

                switch (commonResponse.Status)
                {
                    case AppStatusCode.SUCCESS:
                        return Ok(response.User);
                    case AppStatusCode.RECORD_NOT_FOUND:
                        return NotFound(commonResponse);
                    case AppStatusCode.BAD_REQUEST:
                    case AppStatusCode.INVALID_INPUT:
                        return BadRequest(commonResponse);
                    case AppStatusCode.DATABASE_ERROR:
                        return StatusCode(503, commonResponse);
                    default:
                        return StatusCode(500, commonResponse);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while getting user ");
                return StatusCode(500, new CommonResponse
                {
                    Status = AppStatusCode.INTERNAL_SERVER_ERROR,
                    Message = "An error occurred while fetching user.",
                });
            }
        }
        

    }
}
