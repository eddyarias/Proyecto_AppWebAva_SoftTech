using Microsoft.AspNetCore.Mvc;

namespace GaleriaArte.ObraService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("MediaService is running.");
    }
}