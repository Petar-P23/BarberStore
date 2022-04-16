using BarberStore.Core.Contracts;
using BarberStore.Core.Models.Store;
using BarberStore.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BarberStore.Web.Controllers
{
    public class StoreController : Controller
    {
        private const int DefaultPageSize = 4;
        private const int DefaultPage = 1;
        private readonly IStoreService storeService;
        private readonly UserManager<ApplicationUser> userManager;

        public StoreController(IStoreService storeService,
               UserManager<ApplicationUser> userManager)
        {
            this.storeService = storeService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Explore(int page, int size, string category) //
        {
            if (size <= 0) size = DefaultPageSize;
            if (page <= 0) page = DefaultPage;

            var products = await this.storeService.GetStorePageAsync(page - 1, size, category);
            return this.View(products);
        }
        public async Task<IActionResult> Product(string id, string? errors) //
        {
            if (errors != null)
            {
                return BadRequest(errors);
            }

            var product = await this.storeService.GetProductPageAsync(id);

            return this.View(product);
        }
        [HttpGet]
        public IActionResult AddProductToCart(string returnUrl)
        {
            return Redirect(returnUrl);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddProductToCart(string productId, int quantity) //
        {
            if (quantity == 0) quantity = 1;
            var user = this.userManager.GetUserId(this.User);
            var (success, errors) = await this.storeService.AddProductToCartAsync(user, productId, quantity);
            if (success)
                return RedirectToAction("Explore");

            return this.RedirectToAction("Product", routeValues: new { productId, errors });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PlaceOrder(PlaceOrderProductModel[]? products)
        {
            var user = this.userManager.GetUserId(this.User);

            var (success, errors) = await this.storeService.PlaceOrderAsync(products, user);
            if (!success)
            {
                return BadRequest(errors);
            }

            return RedirectToAction("Index", "Home");
        }
        
    }
}
