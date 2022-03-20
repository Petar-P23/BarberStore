using Microsoft.AspNetCore.Mvc;

namespace BarberStore.Web.Areas.Administration.Controllers
{
    public class StoreController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
