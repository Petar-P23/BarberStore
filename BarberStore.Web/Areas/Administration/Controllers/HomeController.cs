using Microsoft.AspNetCore.Mvc;

namespace BarberStore.Web.Areas.Administration.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
