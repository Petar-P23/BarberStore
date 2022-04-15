using BarberStore.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberStore.Web.Areas.Administration.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        public RolesController(RoleManager<IdentityRole> roleManager, 
            UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public async Task<IActionResult> SeedRoles()
        {
            if(!await this.roleManager.RoleExistsAsync("Admin"))
                await this.roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await this.userManager.Users.AnyAsync(u => u.Email == "admin@test.com"))
            {
                var user = new ApplicationUser()
                {
                    UserName = "admin@test.com",
                    Email = "admin@test.com",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "Admin",
                };

                user.Cart = new Cart { UserId = user.Id };

                await this.userManager.CreateAsync(user, "Admin@123");
            }

            var userIdentity = await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == "admin@test.com");
            await this.userManager.AddToRoleAsync(userIdentity, "Admin");

            return RedirectToAction("Index", "Home");
        }
    }
}
