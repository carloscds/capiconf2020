using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;

namespace APIServico.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly ILogger<ClienteController> _logger;
        private readonly IHttpContextAccessor _httpAcessor;

        public ClienteController(
            IHttpContextAccessor httpAcessor,
            ILogger<ClienteController> logger
            )
        {
            _httpAcessor = httpAcessor;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_httpAcessor.HttpContext.User.Claims.Select(s => new { s.Type, s.Value }));
        }
    }
}
