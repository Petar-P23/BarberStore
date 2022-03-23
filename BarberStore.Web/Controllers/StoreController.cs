using Microsoft.AspNetCore.Mvc;

namespace BarberStore.Web.Controllers
{
    public class StoreController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
