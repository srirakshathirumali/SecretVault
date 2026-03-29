using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecretVault.Application.DTOs;
using SecretVault.Application.Interfaces;

namespace SecretVault.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterResponseDto),StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result= await _authService.RegisterAsync(request);
            return CreatedAtAction(nameof(Register), new {id=result.UserId},result);
        }

    }
}
