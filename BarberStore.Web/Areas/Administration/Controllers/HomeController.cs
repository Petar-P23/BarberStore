using Microsoft.AspNetCore.Mvc;

namespace BarberStore.Web.Areas.Administration.Controllers
{
    public class HomeController : BaseController
    {
        //private readonly RoleManager<IdentityRole> roleManager;

        //public HomeController(RoleManager<IdentityRole> roleManager)
        //{
        //    this.roleManager = roleManager;
        //}

        public IActionResult Index()
        {
            return View();
        }

        //[AllowAnonymous]
        //public async Task<IActionResult> CreateRoles()
        //{
        //    await roleManager.CreateAsync(new IdentityRole("Admin"));

        //    return Ok();
        //}
    }
}
