using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuAsador.Infrastructure.Data;

namespace TuAsador.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpecialtiesController : ControllerBase
{
    private readonly TuAsadorDbContext _db;

    public SpecialtiesController(TuAsadorDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var specialties = await _db.Specialties
            .Select(s => new { s.Id, s.Name })
            .ToListAsync();

        return Ok(specialties);
    }
}
