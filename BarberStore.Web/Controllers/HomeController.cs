using BarberStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BarberStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this._logger = logger;
        }

        public IActionResult Welcome()
        {
            return this.View();
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult AboutUs()
        {
            return this.View();
        }
        public IActionResult Services()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}