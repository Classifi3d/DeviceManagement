using Application.Abstraction;
using Application.CommandsAndQueries.Users;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserController : Controller
{

    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [AllowAnonymous]
    [HttpPost]
    [Route("sign-up")]
    [EnableRateLimiting("registerLimiter")]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserRequestDTO userDto, 
        CancellationToken cancellationToken)
    {
        if (userDto is null)
        {
            return BadRequest("User data is required.");
        }

        var result = await _mediator.Send(new CreateUserCommand(userDto), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();

    }

    
}

