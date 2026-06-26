using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuAsador.Application.Features.Events;
using TuAsador.Application.Features.Events.Commands.Apply;
using TuAsador.Application.Features.Events.Commands.Create;
using TuAsador.Application.Features.Events.Commands.SelectApplication;
using TuAsador.Application.Features.Events.Queries.GetAll;
using TuAsador.Application.Features.Events.Queries.GetApplied;
using TuAsador.Application.Features.Events.Queries.GetApplications;
using TuAsador.Application.Features.Events.Queries.GetById;
using TuAsador.Application.Features.Events.Queries.GetMine;

namespace TuAsador.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _mediator.Send(new GetAllEventsQuery(userId));
        return Ok(result);
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> GetMine()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _mediator.Send(new GetMyEventsQuery(userId));
        return Ok(result);
    }

    [HttpGet("applied")]
    [Authorize(Roles = "Asador")]
    public async Task<IActionResult> GetApplied()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var result = await _mediator.Send(new GetAppliedEventsQuery(userId));
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _mediator.Send(new GetEventByIdQuery(id, userId));

        if (result == null)
            return NotFound(new { message = "Evento no encontrado" });

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _mediator.Send(new CreateEventCommand(
            userId, request.Date, request.Time, request.City, request.Address,
            request.PeopleCount, request.EventType, request.ServiceDesired, request.Notes));
        return Ok(result);
    }

    [HttpPost("{id:guid}/apply")]
    [Authorize(Roles = "Asador")]
    public async Task<IActionResult> Apply(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var result = await _mediator.Send(new ApplyToEventCommand(id, userId));
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/applications")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> GetApplications(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var result = await _mediator.Send(new GetEventApplicationsQuery(id, userId));
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPut("{id:guid}/applications/{applicationId:guid}/select")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> SelectApplication(Guid id, Guid applicationId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var result = await _mediator.Send(new SelectApplicationCommand(id, applicationId, userId));
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
