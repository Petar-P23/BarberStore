using Microsoft.AspNetCore.Mvc;

namespace BarberStore.Web.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
