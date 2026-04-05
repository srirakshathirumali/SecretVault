using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretVault.Domain.Interfaces;

namespace SecretVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]        // Admin only
    public class AuditController : ControllerBase
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditController(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        [HttpGet("logs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetLogs()
        {
            var logs = await _auditLogRepository.GetAllAync();
            return Ok(logs);
        }
    }
}
