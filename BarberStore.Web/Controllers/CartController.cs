using BarberStore.Core.Contracts;
using BarberStore.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BarberStore.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IStoreService storeService;
        private readonly UserManager<ApplicationUser> userManager;

        public CartController(IStoreService storeService,
            UserManager<ApplicationUser> userManager)
        {
            this.storeService = storeService;
            this.userManager = userManager;
        }
        [Authorize]
        public async Task<IActionResult> Cart()
        {
            try
            {
                var user = this.userManager.GetUserId(this.User);
                var cart = await this.storeService.GetCartAsync(user);
                return this.View(cart);
            }
            catch (Exception)
            {
                return base.NotFound();
            }
        }
    }
}
