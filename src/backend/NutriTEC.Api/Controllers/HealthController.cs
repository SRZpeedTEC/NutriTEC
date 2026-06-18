using Microsoft.AspNetCore.Mvc;

namespace NutriTEC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        // Technical endpoint used to confirm that the API is available.
        return Ok(new
        {
            status = "Healthy",
            service = "NutriTEC SQL Server API",
            timestamp = DateTimeOffset.UtcNow
        });
    }
}
