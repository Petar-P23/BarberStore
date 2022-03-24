using System.Linq.Expressions;
using BarberStore.Core.Contracts;
using BarberStore.Infrastructure.Data.Models;
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

        public async Task<IActionResult> GetCart()
        {
            try
            {
                var user = this.userManager.GetUserId(this.User);
                var cart = await this.storeService.GetCart(user);
                return this.Ok(cart);
            }
            catch (Exception)
            {
                return base.NotFound();
            }

        }

        [HttpPost]
        public async Task<IActionResult> AddProductToCart(string productId, int quantity)
        {
                var user = this.userManager.GetUserId(this.User);
                var (success,errors) = await this.storeService.AddProductToCart(user, productId, quantity);

                if (success)
                    return this.Ok();
                else
                    return base.BadRequest(errors);
        }

        
    }
}
