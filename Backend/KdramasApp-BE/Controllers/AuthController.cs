using KdramasApp_BE.DTOs;
using KdramasApp_BE.Services.Interfaces;
using KdramasApp_BE.Enums;
using Microsoft.AspNetCore.Mvc;
namespace KdramasApp_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        this._authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        //call the service
        RegisterResult result = await _authService.RegisterAsync(request);

        //interpret the results and return the status code and message
        
        switch(result.Status)
        {
            case RegisterStatus.RegisterSuccessful:
                return StatusCode(StatusCodes.Status201Created, 
                    new APIResponse("Registration successful"));
            case RegisterStatus.EmailAlreadyExists:
                return Conflict(new 
                APIResponse("There is already an account with the same email"));
            default:
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new APIResponse("An unexpected error has occurred"));
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        //call the service
        LoginResult result = await _authService.LoginAsync(request);

        //interpret the result and return the status code accordingly
        switch(result.Status)
        {
            case LoginStatus.InvalidEmailOrPassword:
                //return StatusCode(StatusCodes.Status401Unauthorized,
                //    new APIResponse("Invalid email or password"));
                return Unauthorized(new LoginResponse("Invalid email or password"));
            case LoginStatus.LoginSuccessful:
                if(string.IsNullOrWhiteSpace(result.Token))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new LoginResponse("An unexpected error has occurred"));
                }
                return Ok(new LoginResponse("Login successful", result.Token));
            default:
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new LoginResponse("An unexpected error has occurred"));
        }
    }
}
