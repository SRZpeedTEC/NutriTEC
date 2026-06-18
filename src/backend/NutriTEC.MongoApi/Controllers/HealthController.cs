using Microsoft.AspNetCore.Mvc;

namespace NutriTEC.MongoApi.Controllers;

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
            service = "NutriTEC MongoDB API",
            timestamp = DateTimeOffset.UtcNow
        });
    }
}
