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
        private const int DefaultPageSize = 20;
        private const int DefaultPage = 1;
        private readonly IStoreService storeService;
        private readonly UserManager<ApplicationUser> userManager;

        public StoreController(IStoreService storeService,
               UserManager<ApplicationUser> userManager)
        {
            this.storeService = storeService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Explore(int page, int size, string category)
        {
            if (size == 0) size = DefaultPageSize;
            if (page == 0) page = DefaultPage;

            var products = await this.storeService.GetStorePage(page - 1, size, category);
            return this.View(products);
        }
        public async Task<IActionResult> Product(string id, string? errors)
        {
            if (errors != null)
            {
                this.ViewBag["error"] = errors;
            }

            var product = await this.storeService.GetProductPage(id);

            return this.View(product);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddProductToCart(string productId, int quantity)
        {
            if (quantity == 0) quantity = 1;
            var user = this.userManager.GetUserId(this.User);
            var (success, errors) = await this.storeService.AddProductToCart(user, productId, quantity);
            if (success)
                return this.Ok();

            return this.RedirectToAction("Product", routeValues: new { productId, errors });
        }
        [Authorize]
        public async Task<IActionResult> PlaceOrder()
        {
            var user = this.userManager.GetUserId(this.User);

            var order = await this.storeService.GetCart(user);
            if (order == null) 
                return this.RedirectToAction("Explore");

            return this.View(order);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PlaceOrder(PlaceOrderModel orderModel)
        {
            var (success, errors) = await this.storeService.PlaceOrder(orderModel);
            if (!success)
            {
                this.ViewBag["error"] = errors;
                return this.View();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
