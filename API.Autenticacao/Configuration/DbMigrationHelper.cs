using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace API.Autenticacao.Configuration
{

    public static class DbMigrationHelpers
    {
        /// <summary>
        /// Generate migrations before running this method, you can use command bellow:
        /// Nuget package manager: Add-Migration DbInit -context ApplicationIdentityContext -output Data/Migrations
        /// Dotnet CLI: dotnet ef migrations add DbInit -c ApplicationIdentityContext -o Data/Migrations
        /// </summary>
        public static async Task EnsureSeedData(IServiceScope serviceScope)
        {
            var services = serviceScope.ServiceProvider;
            await EnsureSeedData(services);
        }

        public static async Task EnsureSeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Create admin role
                if (!await roleManager.RoleExistsAsync("Administrator"))
                {
                    var role = new IdentityRole { Name = "Administrator" };

                    await roleManager.CreateAsync(role);
                }

                // Create admin user
                if (await userManager.FindByNameAsync(configuration["Configuration:DefaultUser"]) != null) return;

                var user = new IdentityUser()
                {
                    UserName = configuration["Configuration:DefaultUser"],
                    Email = configuration["Configuration:DefaultEmail"],
                    EmailConfirmed = true,
                    LockoutEnd = null
                };

                var result = await userManager.CreateAsync(user, configuration["Configuration:DefaultPassword"]);
                await userManager.AddToRoleAsync(user, "Administrator");
            }
        }

    }
}