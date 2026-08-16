using Microsoft.AspNetCore.Mvc;
using MiniSaaS.API.Extensions;
using MiniSaaS.Application.Common.Models;
using MiniSaaS.Application.Tenants.DTOs;
using MiniSaaS.Application.Tenants.Services;

namespace MiniSaaS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantsController( ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResultDto<TenantResponse>),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultDto<TenantResponse>),StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ResultDto<object>),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequest request,CancellationToken cancellationToken)
    {
        var result = await _tenantService.CreateAsync(request,cancellationToken);

        if (!result.Success)
        {
            return this.ToActionResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id },result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ResultDto<TenantResponse>),StatusCodes.Status200OK)]
    [ProducesResponseType( typeof(ResultDto<TenantResponse>),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id,CancellationToken cancellationToken)
    {
        var result = await _tenantService.GetByIdAsync(id,cancellationToken);

        return this.ToActionResult(result);
    }
}
