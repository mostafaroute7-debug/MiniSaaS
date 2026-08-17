using Microsoft.AspNetCore.Mvc;
using MiniSaaS.API.Extensions;
using MiniSaaS.Application.Auth.DTOs;
using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Application.Common.Models;
using MiniSaaS.Infrastructure.MultiTenancy;

namespace MiniSaaS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TenantRequired]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ResultDto<AuthResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<AuthResponse>),StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request,CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(request,cancellationToken);

            return this.ToActionResult(result);
        }
    }
}
