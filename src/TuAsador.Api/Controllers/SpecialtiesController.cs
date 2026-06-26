using MediatR;
using Microsoft.AspNetCore.Mvc;
using TuAsador.Application.Features.Specialties.Queries.GetAll;

namespace TuAsador.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpecialtiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SpecialtiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllSpecialtiesQuery());
        return Ok(result);
    }
}
