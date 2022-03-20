using Microsoft.AspNetCore.Mvc;

namespace BarberStore.Web.Controllers
{
    public class AppointmentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
