using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ambulance.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecureController : ControllerBase
    {
        [HttpGet("profile")]
        public IActionResult profile()
        {
            var user = HttpContext.Items["User"];
            if (user == null)
            {
                return Unauthorized(new { Message = "Token is invalid" });
            }
            return Ok(new { Message = "You are authnticated", User = user });
        }
    }
}
