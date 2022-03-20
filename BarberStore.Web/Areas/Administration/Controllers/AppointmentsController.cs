using Microsoft.AspNetCore.Mvc;

namespace BarberStore.Web.Areas.Administration.Controllers
{
    public class AppointmentsController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
