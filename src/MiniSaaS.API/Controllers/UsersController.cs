using Microsoft.AspNetCore.Mvc;
using MiniSaaS.Application.Common.Models;
using MiniSaaS.Application.Users.DTOs;
using MiniSaaS.Application.Users.Services;

namespace MiniSaaS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController( IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [ProducesResponseType( typeof(ResultDto<PagedResultDto<UserResponse>>),StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationRequest request,CancellationToken cancellationToken)
    {
        var result = await _userService.GetAllAsync(request,cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResultDto<UserResponse>),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultDto<UserResponse>),StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request,CancellationToken cancellationToken)
    {
        var result = await _userService.CreateAsync(request,cancellationToken);

        if (!result.Success)
        {
            return Conflict(result);
        }

        return Created($"/api/users/{result.Data!.Id}",result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ResultDto<UserResponse>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultDto<UserResponse>),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResultDto<UserResponse>),StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id,[FromBody] UpdateUserRequest request,CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateAsync(id,request,cancellationToken);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ResultDto<bool>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultDto<bool>),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
    int id,
    CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteAsync(id,cancellationToken);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
