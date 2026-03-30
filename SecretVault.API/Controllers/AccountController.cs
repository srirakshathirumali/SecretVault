using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecretVault.Application.DTOs.Account;
using SecretVault.Application.Interfaces;
using System.Security.Claims;

namespace SecretVault.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
             _accountService = accountService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(AccountResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequestDto request)
        {
            var userId= GetUserId();
            var account = await _accountService.CreateAccountAsync(request,userId);
            return CreatedAtAction(nameof(CreateAccount), new { id = account.Id }, account);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AccountResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAccounts()
        {
            var userId = GetUserId();
            var result= await _accountService.GetUserAccountsAsync(userId);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(AccountResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAccountById(Guid id)
        {
            var userId = GetUserId();
            var account = await _accountService.GetAccountByAccountId(id, userId);
            return Ok(account);
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }
    }
}
