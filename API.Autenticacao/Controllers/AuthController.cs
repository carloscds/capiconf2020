using API.Autenticacao.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.JwtSigningCredentials.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Autenticacao.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IJsonWebKeySetService _jsonWebKeySetService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(
            ILogger<AuthController> logger,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IJsonWebKeySetService jsonWebKeySetService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
            _jsonWebKeySetService = jsonWebKeySetService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginDTO usuario)
        {
            var user = await _userManager.FindByEmailAsync(usuario.Email);
            if (user is null)
            {
                return BadRequest("Usuario/Senha invalidos!");
            }

            var result = await _signInManager.PasswordSignInAsync(user, usuario.Senha, false, false);
            if (!result.Succeeded)
            {
                return BadRequest("Usuario/Senha invalidos!");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _jsonWebKeySetService.GetCurrent();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("sub",user.Id),
                    new Claim("name", user.UserName),
                    new Claim("scope","api_servico")
                }),
                Issuer = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}",
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = key
            };
            foreach (var role in roles)
            {
                tokenDescriptor.Subject.AddClaim(new Claim("role", role));
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(tokenHandler.WriteToken(token));
        }
    }
}
