using BarberStore.Core.Contracts;
using BarberStore.Core.Models.Store;
using BarberStore.Infrastructure.Data.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BarberStore.Web.Areas.Administration.Controllers
{
    public class StoreController : BaseController
    {
        private readonly IStoreService storeService;

        public StoreController(IStoreService storeService)
        {
            this.storeService = storeService;
        }

        public async Task<IActionResult> Manage()
        {
            IEnumerable<OrderViewModel> orders = await this.storeService.GetAllOrdersByStatus(Status.Pending);
            return this.View(orders);
        }

        public async Task<IActionResult> FinishOrder(string id)
        {
            await this.storeService.MarkOrderAsFinisedAsync(id);
            return RedirectToAction("Manage");
        }
    }
}
