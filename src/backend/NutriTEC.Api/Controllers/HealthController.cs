using Microsoft.AspNetCore.Mvc;

namespace NutriTEC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        // Control tecnico para confirmar que la API esta disponible.
        return Ok(new
        {
            status = "Healthy",
            service = "NutriTEC SQL Server API",
            timestamp = DateTimeOffset.UtcNow
        });
    }
}
