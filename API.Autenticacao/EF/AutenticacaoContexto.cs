using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Autenticacao.EF
{
    public class AutenticacaoContexto : IdentityDbContext
    {
        public AutenticacaoContexto(DbContextOptions<AutenticacaoContexto> options) : base(options) { }

    }
}
