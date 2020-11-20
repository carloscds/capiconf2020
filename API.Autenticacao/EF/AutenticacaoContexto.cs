using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Security.JwtSigningCredentials;
using NetDevPack.Security.JwtSigningCredentials.Store.EntityFrameworkCore;

namespace API.Autenticacao.EF
{
    public class AutenticacaoContexto : IdentityDbContext, ISecurityKeyContext
    {
        public AutenticacaoContexto(DbContextOptions<AutenticacaoContexto> options) : base(options) { }

        public DbSet<SecurityKeyWithPrivate> SecurityKeys { get; set; }
    }
}
